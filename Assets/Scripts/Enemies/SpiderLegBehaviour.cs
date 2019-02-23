using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderLegBehaviour : EnemyPulsing
{
    public float maxRotation;
    public float minRotation;

    private bool shouldLookAtPlayer;

    public bool isDead;

    internal override void DetectEnemyPositionOnGrid()
    {
        //base.DetectEnemyPositionOnGrid();
    }

    internal override void Init()
    {
        base.Init();
        print("initing leg");
        isDead = false;
        shouldFollowPlayer = false;
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

    public void Stopm()
    {
        movementStatus = MovementType.Pulse;
    }

    internal override void PulseAction()
    {
        base.PulseAction();

        //float yLookRotation = transform.localRotation.eulerAngles.y;
        //print("yLookRotation: "+ yLookRotation + " minRotation: " + minRotation + " maxRotation: "+ maxRotation);

        LookAtPlayer(minRotation - 90, maxRotation - 90); // 90 deg compensation for local to global rotation conversion
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyBombTrigger"))
        {
            //override die?
            gameObject.SetActive(false);
            isDead = true;
        }
    }

}
