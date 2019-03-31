using UnityEngine;
using System.Collections;
using SettlersEngine;
using System.Collections.Generic;
using EZCameraShake;
using System;

public class Enemy : MonoBehaviour, IPooledObject {

    internal Point pointPos;
    public Point prevPointPos;
    public float speed;
    public float findPlayerInterval = 0.5f;
    public float damage;

    public float stunnedTimer = 1;
    private float stunnedRemaining;

    public int pointsReward = 10;
    public float deathScoreMultiplier = 0.1f;
    public bool shouldFollowPlayer = true;

    public float minDistanceTargeting;
    public float maxDistanceTargeting;

    public GameObject ZapEffect;

    public Animator animator;

    internal Transform playerObject;

    private Point playerPointPos;
    private LinkedList<GridNode> pathList;

    internal Rigidbody rrigidBody;
    internal float savedDrag;
    private float onFloorYPos = 4f;

    [HideInInspector] public LevelGenerator gridHolder;


    internal MovementType movementStatus;
    internal RigidbodyConstraints constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

    internal enum MovementType { Static, TrackingPlayer, Pushed, Stunned, Falling, Shooting, Pulse }

    /*private void Awake()
    {
        gridHolder = LevelGenerator.Instance;

    }*/

    public void OnObjectSpawn()
    {
        playerPointPos = GameManager.Instance.playerPointPosition;
        playerObject = GameManager.Instance.PlayerInstance.transform;
        gridHolder = LevelGenerator.Instance;
        DetectEnemyPositionOnGrid();

        StartCoroutine(FindPathEverySeconds(findPlayerInterval));

        movementStatus = MovementType.Static;
        rrigidBody = GetComponent<Rigidbody>();

        savedDrag = rrigidBody.drag;
        rrigidBody.drag = 0;

        Init();
    }

    internal virtual void Init() { }

    IEnumerator FindPathEverySeconds(float t)
    {
        while (true)
        {
            if (shouldFollowPlayer)
            {
                playerPointPos = GameManager.Instance.playerPointPosition;
                //GridNode[,] grid = gridHolder.GetGridPortion(pointPos, (int)maxDistanceTargeting * 2, (int)maxDistanceTargeting * 2);
                GridNode[,] grid = gridHolder.GetGrid();
                SpatialAStar<GridNode, GridNode> aStar = new SpatialAStar<GridNode, GridNode>(grid);
                if (playerPointPos != null)
                    pathList = aStar.Search(pointPos, playerPointPos, null);
                else
                    Debug.LogWarning("Enemy trying to pathfind the player but his pos is null.");
            }
            yield return new WaitForSeconds(t);
        }
    }

    internal virtual void DetectEnemyPositionOnGrid()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            if (hit.transform.CompareTag("FloorCube") || hit.transform.CompareTag("WallCube"))
            {
                //if the old pointpos is occupied, set it to normal.
                if(pointPos != null && gridHolder.GetGridNodeType(pointPos.x, pointPos.y) == TileType.Occupied)
                    gridHolder.SetGridNodeType(pointPos.x, pointPos.y, TileType.Normal);
                //then set a new pointpos
                string name = hit.transform.parent.name;
                string[] posArr = name.Split(',');

                prevPointPos = pointPos;
                pointPos = new Point(int.Parse(posArr[0]), int.Parse(posArr[1]));

                if (gridHolder.GetGridNodeType(pointPos.x, pointPos.y) == TileType.Pit)
                {
                    Die();
                }
                else
                {
                    gridHolder.SetGridNodeType(pointPos.x, pointPos.y, TileType.Occupied);
                }
            }
        }

        if (rrigidBody != null)
        {
            if (transform.position.y <= onFloorYPos && rrigidBody.drag == 0)
            {
                //print("enemy landed on the floor");
                rrigidBody.drag = savedDrag;
                rrigidBody.constraints = constraints;
            }
        }
    }

    public void Die()
    {
        movementStatus = MovementType.Falling;
        PowerUpObject powerup = PowerupFactory.Instance.RollPowerup();
        if (powerup != null && prevPointPos != null)
        {
            //TODO: instantiate powerup from pool
            Vector3 pos = gridHolder.GetGridNode(prevPointPos.x, prevPointPos.y).GetGameNodeRef().transform.position;
            Vector3 actualPos = new Vector3(pos.x, onFloorYPos -1, pos.z);
            Instantiate(powerup.prefab, actualPos, powerup.prefab.transform.rotation);
            print("creaing a powerup! "+powerup.name);
        }
    }

    void Update()
    {
        DetectEnemyPositionOnGrid();
        //if the enemy is over a pit, fall down


        switch (movementStatus)
        {
            case MovementType.Static:
                StaticAction();
                break;

            case MovementType.TrackingPlayer:
                TrackingAction();
                LookAtPlayer();
                break;

            case MovementType.Shooting:
                LookAtPlayer();
                ShootingAction();
                break;

            case MovementType.Pulse:
                PulseAction();
                break;

            case MovementType.Pushed:
                //recover from push and continue following the player
                if (rrigidBody.velocity.x < 0.1f && rrigidBody.velocity.y < 0.1f)
                {
                    movementStatus = MovementType.Stunned;
                    stunnedRemaining = stunnedTimer;
                }
                break;

            case MovementType.Stunned:
                StunnedAction();
                break;

            case MovementType.Falling:
                print("Enemy is falling...");
                GameManager.Instance.AddScoreMultiplier(deathScoreMultiplier);
                GameManager.Instance.AddScore(pointsReward);
                //CameraShaker.Instance.ShakeOnce(2f, 4f, 0.1f, 1f);
                GameManager.Instance.IncrementEnemyKillCount();
                gameObject.SetActive(false);
                break;
        }
    }

    internal virtual void PulseAction() { }

    internal virtual void StunnedAction()
    {
        stunnedRemaining -= Time.deltaTime;
        if (stunnedRemaining <= 0)
            movementStatus = MovementType.TrackingPlayer;
    }

    internal virtual void ShootingAction() {}

    internal virtual void StaticAction()
    {
        float distanceFromPlayer = Vector3.Distance(transform.position, playerObject.position);
        if (distanceFromPlayer > minDistanceTargeting && distanceFromPlayer < maxDistanceTargeting)
        {
            //if (pathList != null && pathList.Count > 0)
            {
                movementStatus = MovementType.TrackingPlayer;
            }
        }
        else
        {
            shouldFollowPlayer = false;
        }
    }

    internal virtual void TrackingAction()
    {
        shouldFollowPlayer = true;

        float distanceFromPlayer = Vector3.Distance(transform.position, playerObject.position);
        if (distanceFromPlayer < minDistanceTargeting || distanceFromPlayer > maxDistanceTargeting)
        {
            movementStatus = MovementType.Static;
            return;
        }

        if (pathList == null || pathList.Count == 0)
        {
            movementStatus = MovementType.Static;
            return;
        }

        Vector3 targetPos = pathList.First.Value.GetGameNodeRef().transform.position;
        targetPos.y = transform.position.y;
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

        if (IsNear(targetPos, transform.position, 0.5f))
        {
            pathList.RemoveFirst();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (ZapEffect != null)
                StartCoroutine(Zap(0.5f));
        }
        /*if (collision.gameObject.CompareTag("FloorCube"))
        {
            //print("enemy colides with floor tile");
            rrigidBody.drag = savedDrag;
        }*/
    }

    private IEnumerator Zap(float duration)
    {
        ZapEffect.SetActive(true);
        yield return new WaitForSeconds(duration);
        ZapEffect.SetActive(false);
    }

    internal virtual void LookAtPlayer(float minClamp=0, float maxClamp=0)
    {
        Vector3 lookPosition = new Vector3(playerObject.position.x, transform.position.y, playerObject.position.z);
        //transform.LookAt(lookPosition);

        Vector3 direction = lookPosition - transform.position;
        Quaternion toRotation = Quaternion.LookRotation(direction);
        toRotation = Quaternion.Lerp(transform.rotation, toRotation, 0.3f);

        if (minClamp != 0 && maxClamp != 0)
        {
            Vector3 er = toRotation.eulerAngles;
            //print(" yRot: "+er.y +" "+gameObject.name);
            float clampedY = Mathf.Clamp(er.y, minClamp, maxClamp);
            toRotation.eulerAngles = new Vector3(er.x, clampedY, er.z);
        }
        transform.rotation = toRotation;
    }

    public virtual void ForcePush(Vector3 direction, float force)
    {
        movementStatus = MovementType.Pushed;
        rrigidBody.AddForce(direction * force);
    }

    private bool IsNear(Vector3 pos1, Vector3 pos2, float threshold)
    {
        if (Vector3.Distance(pos1, pos2) < threshold)
            return true;
        return false;
    }
}
