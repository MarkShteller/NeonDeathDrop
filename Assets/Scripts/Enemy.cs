using UnityEngine;
using System.Collections;
using SettlersEngine;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {

    Point pointPos;
    public float speed;
    public float findPlayerInterval = 0.5f;
    public float damage;

    private Point playerPointPos;
    private bool isMovingTowardsPlayer;
    private LinkedList<GridNode> pathList;

    private Vector3 pushedTargetPos;

    private Rigidbody rigidBody;
    public TerrainManager gridHolder;


    private MovementType movementStatus;
    enum MovementType { Static, TrackingPlayer, Pushed, Falling }

    void Start()
    {
        playerPointPos = GameManager.Instance.playerPointPosition;
        isMovingTowardsPlayer = true;
        gridHolder = TerrainManager.Instance;//GameObject.FindGameObjectWithTag("TerrainManager").GetComponent<TerrainBuilder>();
        DetectEnemyPositionOnGrid();
        StartCoroutine(FindPathEverySeconds(findPlayerInterval));
        movementStatus = MovementType.Static;
        rigidBody = GetComponent<Rigidbody>();
    }

    IEnumerator FindPathEverySeconds(float t)
    {
        while (true)
        {
            playerPointPos = GameManager.Instance.playerPointPosition;
            GridNode[,] grid = gridHolder.GetGrid();
            SpatialAStar<GridNode, GridNode> aStar = new SpatialAStar<GridNode, GridNode>(grid);
            pathList = aStar.Search(pointPos, playerPointPos, null);
            yield return new WaitForSeconds(t);
        }
    }

    private void DetectEnemyPositionOnGrid()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            if (hit.transform.tag == "FloorCube")
            {
                //if the old pointpos is occupied, set it to normal.
                if(pointPos != null && gridHolder.GetGridNodeType(pointPos.x, pointPos.y) == GridNode.TileType.Occupied)
                    gridHolder.SetGridNodeType(pointPos.x, pointPos.y, GridNode.TileType.Normal);
                //then set a new pointpos
                string name = hit.transform.name;
                string[] posArr = name.Split(',');
                pointPos = new Point(int.Parse(posArr[0]), int.Parse(posArr[1]));

                if (gridHolder.GetGridNodeType(pointPos.x, pointPos.y) == GridNode.TileType.Pit)
                {
                    movementStatus = MovementType.Falling;
                }
                else
                    gridHolder.SetGridNodeType(pointPos.x, pointPos.y, GridNode.TileType.Occupied);
            }
        }
    }

    void Update()
    {
        DetectEnemyPositionOnGrid();
        //if the enemy is over a pit, fall down
        
        switch (movementStatus)
        {
            case MovementType.Static:
                if (pathList != null && pathList.Count > 0)
                {
                    movementStatus = MovementType.TrackingPlayer;
                }
                break;

            case MovementType.TrackingPlayer:
                if (pathList == null || pathList.Count == 0)
                {
                    movementStatus = MovementType.Static;
                    break;
                }
                /*if (pathList == null)
                    print("pathlist is null");
                if (pathList.First == null)
                    print("pathlist.first is null");
                if (pathList.First.Value == null)
                    print("pathList.First.Value is null");*/
                Vector3 targetPos = pathList.First.Value.GetGameNodeRef().transform.position;
                targetPos.y = transform.position.y;
                float step = speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

                if (IsNear(targetPos, transform.position, 0.5f))
                {
                    pathList.RemoveFirst();
                }

                break;
            case MovementType.Pushed:
                //recover from push and continue following the player
                if (rigidBody.velocity.x < 0.1f && rigidBody.velocity.y < 0.1f)
                {
                    movementStatus = MovementType.TrackingPlayer;
                }
                break;
            case MovementType.Falling:
                print("Enemy is falling...");
                Destroy(gameObject);
                break;
        }
    }

    public void ForcePush(Vector3 direction, float force)
    {
        movementStatus = MovementType.Pushed;
        rigidBody.AddForce(direction * force);
    }

    private bool IsNear(Vector3 pos1, Vector3 pos2, float threshold)
    {
        if (Vector3.Distance(pos1, pos2) < threshold)
            return true;
        return false;
    }
}
