﻿using UnityEngine;
using System.Collections;
using SettlersEngine;
using System.Collections.Generic;
using EZCameraShake;
using System;

public class Enemy : MonoBehaviour, IPooledObject {

    Point pointPos;
    public float speed;
    public float findPlayerInterval = 0.5f;
    public float damage;

    public float stunnedTimer = 1;
    private float stunnedRemaining;

    public int pointsReward = 10;
    public bool shouldFollowPlayer = true;

    public float minDistanceTargeting;
    public float maxDistanceTargeting;

    internal Transform playerObject;

    private Point playerPointPos;
    private LinkedList<GridNode> pathList;

    //private Vector3 pushedTargetPos;

    private Rigidbody rigidBody;
    [HideInInspector] public LevelGenerator gridHolder;


    internal MovementType movementStatus;
    internal enum MovementType { Static, TrackingPlayer, Pushed, Stunned, Falling, Shooting }

    public void OnObjectSpawn()
    {
        playerPointPos = GameManager.Instance.playerPointPosition;
        playerObject = GameManager.Instance.PlayerInstance.transform;
        gridHolder = LevelGenerator.Instance;
        DetectEnemyPositionOnGrid();

        StartCoroutine(FindPathEverySeconds(findPlayerInterval));

        movementStatus = MovementType.Static;
        rigidBody = GetComponent<Rigidbody>();

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
                pathList = aStar.Search(pointPos, playerPointPos, null);
            }
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
                if(pointPos != null && gridHolder.GetGridNodeType(pointPos.x, pointPos.y) == TileType.Occupied)
                    gridHolder.SetGridNodeType(pointPos.x, pointPos.y, TileType.Normal);
                //then set a new pointpos
                string name = hit.transform.name;
                string[] posArr = name.Split(',');
                pointPos = new Point(int.Parse(posArr[0]), int.Parse(posArr[1]));

                if (gridHolder.GetGridNodeType(pointPos.x, pointPos.y) == TileType.Pit)
                {
                    movementStatus = MovementType.Falling;
                }
                else
                    gridHolder.SetGridNodeType(pointPos.x, pointPos.y, TileType.Occupied);
            }
        }
    }

    void Update()
    {
        DetectEnemyPositionOnGrid();
        //if the enemy is over a pit, fall down

        LookAtPlayer();

        switch (movementStatus)
        {
            case MovementType.Static:
                StaticAction();
                break;

            case MovementType.TrackingPlayer:
                TrackingAction();
                break;

            case MovementType.Shooting:
                ShootingAction();
                break;

            case MovementType.Pushed:
                //recover from push and continue following the player
                if (rigidBody.velocity.x < 0.1f && rigidBody.velocity.y < 0.1f)
                {
                    movementStatus = MovementType.Stunned;
                    stunnedRemaining = stunnedTimer;
                }
                break;

            case MovementType.Stunned:
                stunnedRemaining -= Time.deltaTime;
                if (stunnedRemaining <= 0)
                    movementStatus = MovementType.TrackingPlayer;
                break;

            case MovementType.Falling:
                print("Enemy is falling...");
                GameManager.Instance.AddScore(pointsReward);
                CameraShaker.Instance.ShakeOnce(2f, 4f, 0.1f, 1f);
                gameObject.SetActive(false);
                break;
        }
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

    internal virtual void LookAtPlayer() {}

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
