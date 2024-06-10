﻿using UnityEngine;
using System.Collections;
using SettlersEngine;
using System.Collections.Generic;
using EZCameraShake;
using System;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

public class Enemy : MonoBehaviour, IPooledObject {

    internal Point pointPos;
    public Point prevPointPos;
    public float speed;

    

    public float findPlayerInterval = 0.5f;
    public float damage;

   

    protected float stunnedTimer;
    public float pushStunTimer = 1;
    public float dashStunTimer = 2;
    public float heavyStunTimer = 3;
    private float stunnedRemaining;
    [HideInInspector] public bool isSuperStunned = false;
    private float stunCounter =0;
    public float superStunThreshhold =4;

    public int pointsReward = 10;
    public float deathScoreMultiplier = 0.1f;
    public bool shouldFollowPlayer = true;

    public float minDistanceTargeting;
    public float maxDistanceTargeting;

    public bool shouldEvadePlayerPits;

    public AnimationCurve launchedYCurve;
    public float launchAirDuration;
    private float launchAirTime = 0;

    public float attackIntervalTimer = 3;
    private float currentAttackTimer=0;

    public GameObject ZapEffect;
    public VisualEffect stunnedEffect;
    public Animator animator;

    internal Transform playerObject;

    private Point playerPointPos;
    private LinkedList<GridNode> pathList;

    private Vector3 targetMovePos;

    internal Rigidbody rrigidBody;
    internal float savedDrag;
    private float onFloorYPos = 4f;

    private float collisionCooldown = 0.2f;
    private float currentCollisionCooldown;

    [HideInInspector] public LevelGenerator gridHolder;


    internal MovementType movementStatus;
    internal RigidbodyConstraints constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

    internal enum MovementType { Static, TrackingPlayer, Pushed, Stunned, SuperStunned, Launched, Falling, Shooting, Bounce, Pulse, Dead,
        HoleFlying,
        Attacking
    }
    public enum DeathType { Pit, PlayerPit, EnemyPit, Shockwave }
    internal PlayerBehaviour.PlayerAttackType lastAttackType;
    /*private void Awake()
    {
        gridHolder = LevelGenerator.Instance;

    }*/

    public void OnObjectSpawn()
    {
        playerPointPos = GameManager.Instance.playerPointPosition;
        playerObject = GameManager.Instance.PlayerInstance.transform;
        //gridHolder = GameManager.Instance.GetCurrentSublevel();//LevelGenerator.Instance;
        //DetectEnemyPositionOnGrid();


        movementStatus = MovementType.Static;
        rrigidBody = GetComponent<Rigidbody>();

        savedDrag = rrigidBody.drag;
        rrigidBody.drag = 0;

        Init();
    }

    public void SetGridHolder(LevelGenerator sublevel)
    {
        //print("### setting grid");
        gridHolder = sublevel;
        DetectEnemyPositionOnGrid();
        StartCoroutine(FindPathEverySeconds(findPlayerInterval));
    }

    internal virtual void Init() { }

    IEnumerator FindPathEverySeconds(float t)
    {
        yield return null;
        //yield return null;
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
        if (Physics.Raycast(transform.position, Vector3.down, out hit, gridHolder.GetDistanceToPit()))
        {
            if (hit.transform.CompareTag("FloorCube") || hit.transform.CompareTag("WallCube") || hit.transform.CompareTag("WeakCube") || hit.transform.CompareTag("PitCube"))
            {
                //if the old pointpos is occupied, set it to normal.
                if(pointPos != null && gridHolder.GetGridNodeType(pointPos.x, pointPos.y) == TileType.Occupied)
                    gridHolder.SetGridNodeType(pointPos.x, pointPos.y, TileType.Normal);
                //then set a new pointpos
                string name = hit.transform.parent.name;
                string[] posArr = name.Split(',');

                prevPointPos = pointPos;
                pointPos = new Point(int.Parse(posArr[0]), int.Parse(posArr[1]));


                GridNode gNode = gridHolder.GetGridNode(pointPos.x, pointPos.y);

                if (gNode.GetTileType() == TileType.Weak)
                {
                    print("## enemy weak tile broke");
                    gNode.GetGameNodeRef().GetComponentInChildren<WeakTileBehaviour>().StepOnTile(() => gNode.SetType(TileType.Pit));
                }
                else if (gNode.GetTileType() == TileType.Pit)
                {
                    Die(DeathType.Pit);
                }
                else if (gNode.GetTileType() == TileType.EnemyPit)
                {
                    Die(DeathType.EnemyPit);
                }
                else if (gNode.GetTileType() == TileType.PlayerPit)
                {
                    Die(DeathType.PlayerPit);
                }
                else
                {
                    gNode.SetType(TileType.Occupied);
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

    public void Die(DeathType deathType)
    {
        print("enemy died. death type: "+ deathType.ToString());
        if (movementStatus != MovementType.Dead)
        {
            //movementStatus = MovementType.Falling;
            DyingAction();
            PowerUpObject powerup = PowerupFactory.Instance.RollPowerup();
            if (powerup != null && prevPointPos != null)
            {
                //TODO: instantiate powerup from pool
                Vector3 pos = gridHolder.GetGridNode(prevPointPos.x, prevPointPos.y).GetGameNodeRef().transform.position;
                Vector3 actualPos = new Vector3(pos.x, onFloorYPos - 1, pos.z);
                Instantiate(powerup.prefab, actualPos, powerup.prefab.transform.rotation);
                print("creaing a powerup! " + powerup.name);
            }
        }

        float killpoints =0;
        switch (deathType)
        {
            case DeathType.Pit:
                killpoints = 1;
                break;
            case DeathType.PlayerPit:
                killpoints = 3;
                break;
            case DeathType.EnemyPit:
                killpoints = 2f;
                break;
            case DeathType.Shockwave:
                killpoints = 1;
                break;
        }
        switch (lastAttackType)
        {
            case PlayerBehaviour.PlayerAttackType.None:
                break;
            case PlayerBehaviour.PlayerAttackType.Push:
                break;
            case PlayerBehaviour.PlayerAttackType.Pull:
                killpoints += 2;
                break;
            case PlayerBehaviour.PlayerAttackType.Dash:
                killpoints += 1;
                break;
            case PlayerBehaviour.PlayerAttackType.Heavy:
                killpoints += 1;
                break;
            case PlayerBehaviour.PlayerAttackType.ParryBullet:
                killpoints += 3;
                break;
        }

        SpawnMoney(killpoints);

        GameManager.Instance.AddKillPoints(killpoints);
        GameManager.Instance.AddScore(pointsReward);
    }

    private void SpawnMoney(float pointMultiplier)
    {
        GameObject moneyObject = ObjectPooler.Instance.SpawnFromPool("MoneyEffect", transform.position, Quaternion.identity);
        VisualEffect moneyVFX = moneyObject.GetComponent<VisualEffect>();
        moneyVFX.SetVector3("PlayerPosition", playerObject.transform.position);
        StartCoroutine(MoneyTrackPlayer(moneyVFX, 6f));
        
        moneyVFX.SetVector3("SpawnPosition", transform.position);
        moneyVFX.SetInt("Amount", (int)(5 * pointMultiplier));
        moneyVFX.Play();
        //List<VFXBinderBase> binders = (List<VFXBinderBase>)moneyObject.GetComponent<VFXPropertyBinder>().GetPropertyBinders<VFXBinderBase>();
        //binders[0].UpdateBinding()
    }

    private IEnumerator MoneyTrackPlayer(VisualEffect money, float time)
    {
        float t = time;
        while (t > 0)
        {
            t -= Time.deltaTime;
            //print("money tracking player: "+ playerObject.transform.position);
            money.SetVector3("PlayerPosition", playerObject.transform.position);
            yield return null;
        }
    }

    public void UpdateEnemy(bool isTrackPlayer = true)
    {
        DetectEnemyPositionOnGrid();
        //if the enemy is over a pit, fall down


            //print(movementStatus.ToString());
        switch (movementStatus)
        {
            case MovementType.Static:
                StaticAction();
                break;

            case MovementType.TrackingPlayer:
                LookAtPlayer();
                if(isTrackPlayer)
                    TrackingAction();
                break;

            case MovementType.Attacking:
                MeleeAttackAction();
                break;

            case MovementType.Shooting:
                LookAtPlayer();
                ShootingAction();
                break;

            case MovementType.Pulse:
                PulseAction();
                break;

            case MovementType.Pushed:
                PushedAction();
                break;

            case MovementType.Stunned:
                StunnedAction();
                break;
            case MovementType.SuperStunned:
                SuperStunnedAction();
                break;

            case MovementType.Launched:
                LaunchedAction();
                break;

            case MovementType.HoleFlying:
                HoleFlyingAction();
                break;

            case MovementType.Falling:
                //DyingAction();
                break;

            case MovementType.Dead:
                EnemyManager.Instance.RemoveFromActiveEnemies(this);
                break;
        }

        if (currentCollisionCooldown > 0)
            currentCollisionCooldown -= Time.deltaTime;
    }

    internal virtual void PushedAction()
    {
        //recover from push and continue following the player
        if (rrigidBody.velocity.x < 0.05f && rrigidBody.velocity.y < 0.05f)
        {
            if (!isSuperStunned)
                movementStatus = MovementType.Stunned;
            else
                movementStatus = MovementType.SuperStunned;

            stunnedRemaining = stunnedTimer;
        }
    }

    private void MeleeAttackAction()
    {
        currentAttackTimer -= Time.deltaTime;
        LookAtPlayer();

        if (currentAttackTimer <= 0)
        {
            print("Enemy melee attacking!");
            animator.SetTrigger("Attack");
            currentAttackTimer = attackIntervalTimer;
            StartCoroutine(AttackCoroutine(0.5f));
            movementStatus = MovementType.TrackingPlayer;
            return;
        }
        
        float distanceFromPlayer = Vector3.Distance(transform.position, playerObject.position);
        if (distanceFromPlayer > minDistanceTargeting)
        {
            movementStatus = MovementType.TrackingPlayer;
            currentAttackTimer = 0;
        }
    }

    private IEnumerator AttackCoroutine(float time)
    {
        float t = 0;
        Vector3 targetPos = playerObject.transform.position;
        while (t <= time)
        {
            targetPos.y = transform.position.y;
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
            t += Time.deltaTime;
            yield return null;
        }
    }

    public void FlyToHole(Vector3 holePosition)
    {
        targetMovePos = holePosition;
        movementStatus = MovementType.HoleFlying;
    }

    private void HoleFlyingAction()
    {
        targetMovePos.y = transform.position.y;
        float step = speed * Time.deltaTime *3;
        transform.position = Vector3.Lerp(transform.position, targetMovePos, step);
        if (Vector3.Distance(targetMovePos, transform.position) < 0.1f)
            Die(DeathType.PlayerPit);
    }

    private void LaunchedAction()
    {
        if (launchAirTime <= launchAirDuration)
        {
            float yPos = launchedYCurve.Evaluate(launchAirTime / launchAirDuration);
            transform.position = new Vector3(transform.position.x, (onFloorYPos -0.6f) + yPos, transform.position.z);
            launchAirTime += Time.deltaTime;
        }
        else
        {
            movementStatus = MovementType.TrackingPlayer;
        }
    }

    internal virtual void DyingAction()
    {
        //print("Enemy is falling...");
        
        //CameraShaker.Instance.ShakeOnce(2f, 4f, 0.1f, 1f);
        GameManager.Instance.IncrementEnemyKillCount();
        //gameObject.SetActive(false);
        animator.SetTrigger("Dead");
        FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.EnemyFall, transform.position);
        
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = rb.velocity / 3;

        GetComponent<Collider>().enabled = false;
        movementStatus = MovementType.Dead;
    }

    internal virtual void PulseAction() { }

    internal virtual void StunnedAction()
    {
        stunnedRemaining -= Time.deltaTime;
        if (stunnedRemaining <= 0)
        {
            if(stunnedEffect != null)
                stunnedEffect.gameObject.SetActive(false);
            movementStatus = MovementType.TrackingPlayer;
            //stunnedTimer = 0;
        }
    }

    internal virtual void SuperStunnedAction()
    {
        stunnedRemaining -= Time.deltaTime;
        if(stunnedEffect != null)
            stunnedEffect.gameObject.SetActive(true);

        if (stunnedRemaining <= 0)
        {
            stunnedEffect.gameObject.SetActive(false);
            movementStatus = MovementType.TrackingPlayer;
            isSuperStunned = false;
            stunnedTimer = 0;
        }
    }

    internal virtual void ShootingAction() {}

    internal virtual void StaticAction()
    {
        animator.SetBool("Moving", false);

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
        animator.SetBool("Moving", true);

        float distanceFromPlayer = Vector3.Distance(transform.position, playerObject.position);

        if (distanceFromPlayer <= minDistanceTargeting)
        {
            movementStatus = MovementType.Attacking;
            return;
        }
        if (distanceFromPlayer > maxDistanceTargeting)
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

        if (currentCollisionCooldown <= 0)
        {

            /*if (collision.gameObject.CompareTag("Enemy"))
            {
                currentCollisionCooldown = collisionCooldown;
                print("## enemy collision current state: " + movementStatus + " collision: " + collision.gameObject.name);
                if (movementStatus == MovementType.Pushed || movementStatus == MovementType.Stunned)// || movementStatus == MovementType.SuperStunned)
                {
                    TransferVelocity(collision.gameObject);
                }
            }*/

            if (collision.gameObject.CompareTag("WallCube"))
            {
                currentCollisionCooldown = collisionCooldown;
                //print("enemy hitting wall velocity " + rrigidBody.velocity.normalized);
                if (movementStatus == MovementType.Pushed || movementStatus == MovementType.Stunned || movementStatus == MovementType.SuperStunned)
                {
                    FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.EnemyTakePushHit, transform.position);

                    //Vector3 dir = rrigidBody.velocity.normalized * GameManager.Instance.PlayerInstance.currentPushForce * 0.5f;
                    //Vector2 v2dir = Vector2.Perpendicular(rrigidBody.velocity.normalized);
                    //ForcePush(new Vector3(v2dir.x, 0, v2dir.y), GameManager.Instance.PlayerInstance.currentPushForce, PlayerBehaviour.PlayerAttackType.Push);
                    //ForcePush(-rrigidBody.velocity.normalized, GameManager.Instance.PlayerInstance.currentPushForce, PlayerBehaviour.PlayerAttackType.Push);
                }
            }
        }
    }

    private void TransferVelocity(GameObject other)
    {
        print("## transfer velocity");
        other.GetComponent<Enemy>().ForcePush(rrigidBody.velocity.normalized, GameManager.Instance.PlayerInstance.currentPushForce, PlayerBehaviour.PlayerAttackType.Push);
        this.rrigidBody.velocity = Vector3.zero;
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

    public void Launch()
    {
        print("enemy launched");
        movementStatus = MovementType.Launched;
        animator.SetTrigger("TakeHit");
        isSuperStunned = false;
        launchAirTime = 0;
        stunnedRemaining = 0;
        stunCounter = 0;
        stunnedEffect.gameObject.SetActive(false);
        FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.EnemyTakeDashHit, transform.position);
    }

    public virtual void ForcePush(Vector3 direction, float force, PlayerBehaviour.PlayerAttackType attackType, bool superStun = false)
    {
        rrigidBody.AddForce(direction * force);
        animator.SetTrigger("TakeHit");

        lastAttackType = attackType;
        switch (attackType)
        {
            case PlayerBehaviour.PlayerAttackType.Push:
                stunnedTimer += pushStunTimer;
                stunCounter += pushStunTimer;
                FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.EnemyTakePushHit, transform.position);
                break;
            case PlayerBehaviour.PlayerAttackType.Dash:
                stunnedTimer += dashStunTimer;
                stunCounter += dashStunTimer;
                FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.EnemyTakeDashHit, transform.position);
                break;
            case PlayerBehaviour.PlayerAttackType.Heavy:
                stunnedTimer += heavyStunTimer;
                stunCounter += heavyStunTimer;
                // need heavy hit sound
                FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.EnemyTakePushHit, transform.position);
                break;
            case PlayerBehaviour.PlayerAttackType.ParryBullet:
                stunnedTimer += pushStunTimer;
                stunCounter += pushStunTimer;
                FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.EnemyTakePushHit, transform.position);
                break;
        }
        if (stunCounter >= superStunThreshhold)
        {
            isSuperStunned = true;
            stunCounter = 0;
        }
        //else
            //isSuperStunned = false;
        movementStatus = MovementType.Pushed;
    }

    public void DeathEvent()
    {
        gameObject.SetActive(false);
    }

    internal virtual void FireEvent()
    {
    }

    internal virtual void PulseTrigger()
    {
        //throw new NotImplementedException();
    }

    public Point GetPointPos()
    {
        return pointPos;
    }

    private bool IsNear(Vector3 pos1, Vector3 pos2, float threshold)
    {
        if (Vector3.Distance(pos1, pos2) < threshold)
            return true;
        return false;
    }

    public int GetIsTrackingPlayer()
    {
        if (movementStatus == MovementType.TrackingPlayer)
        { return 1; }
        else return 0;
    }
}
