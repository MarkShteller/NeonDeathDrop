using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderLegBehaviour : EnemyPulsing
{
    public float maxRotation;
    public float minRotation;

    private bool shouldLookAtPlayer;

    public bool isDead;
    public bool isActive; 

    internal override void DetectEnemyPositionOnGrid()
    {
        //base.DetectEnemyPositionOnGrid();
    }

    internal override void Init()
    {
        base.Init();
        print("initing leg");
        isDead = false;
        isActive = true;
        shouldFollowPlayer = false;
        shakeScreen = true;
        playerObject = GameManager.Instance.PlayerInstance.transform;
        //gridHolder = LevelGenerator.Instance;

        EnemyManager.Instance.AddBossLegToActiveList(this);

        movementStatus = MovementType.Static;
        rrigidBody = GetComponent<Rigidbody>();

        savedDrag = rrigidBody.drag;
        rrigidBody.drag = 0;
    }


    internal override void StaticAction()
    {
        if (isActive)
        {
            float distanceFromPlayer = Vector3.Distance(transform.position, playerObject.position);
            if (distanceFromPlayer < pulseDistanceFromPlayer)
            {
                movementStatus = MovementType.Pulse;
            }
        }
    }

    public override void ForcePush(Vector3 direction, float force, PlayerBehaviour.PlayerAttackType attackType, bool superStun = false)
    {
        animator.SetTrigger("Pushed");
    }

    public void ClingForYourLife(Transform FinalLegPoint)
    {
        print("Leg clinging for its life!");
        movementStatus = MovementType.Static;
        isActive = false;
        transform.position = FinalLegPoint.position;
        transform.rotation = FinalLegPoint.rotation;
        animator.SetTrigger("Cling");
    }

    public void Stopm()
    {
        movementStatus = MovementType.Pulse;
    }

    internal override void PulseAction()
    {
        base.PulseAction();
        print("## spider leg pulse");
        //float yLookRotation = transform.localRotation.eulerAngles.y;
        //print("yLookRotation: "+ yLookRotation + " minRotation: " + minRotation + " maxRotation: "+ maxRotation);

        LookAtPlayer(minRotation - 90, maxRotation - 90); // 90 deg compensation for local to global rotation conversion
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyBombTrigger"))
        {
            //override die?
            //gameObject.SetActive(false);
            animator.SetTrigger("Dead");
            isDead = true;
        }
    }

    public void Dead()
    {
        gameObject.SetActive(false);
    }

}
