using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderLegBehaviour : EnemyPulsing
{
    internal override void DetectEnemyPositionOnGrid()
    {
        //base.DetectEnemyPositionOnGrid();
    }

    internal override void Init()
    {
        base.Init();
        print("initing leg");
        playerObject = GameManager.Instance.PlayerInstance.transform;
        gridHolder = LevelGenerator.Instance;

        movementStatus = MovementType.Static;
        rrigidBody = GetComponent<Rigidbody>();

        savedDrag = rrigidBody.drag;
        rrigidBody.drag = 0;
    }


    internal override void StaticAction()
    {
        float distanceFromPlayer = Vector3.Distance(transform.position, playerObject.position);
        if (distanceFromPlayer < pulseDistanceFromPlayer)
        {
            movementStatus = MovementType.Pulse;
        }
    }

}
