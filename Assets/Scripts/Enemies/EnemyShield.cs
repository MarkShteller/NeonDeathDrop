using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShield : EnemyPulsing
{
    //public float pushedTimer;
    public GameObject shieldObject;
    private bool isShielded = true;

    internal override void Init()
    {
        base.Init();
        shieldObject.SetActive(true);
        isShielded = true;
    }

    /*internal override void StunnedAction()
    {
        rrigidBody.velocity = Vector3.zero;
        base.StunnedAction();
    }*/

    public override void ForcePush(Vector3 direction, float force, PlayerBehaviour.PlayerAttackType attackType, bool superStun = false)
    {
        if (!isShielded)
        {
            print("# not shielded - regular force push"); 
            base.ForcePush(direction, force, attackType, superStun);
            return;
        }

        //rrigidBody.AddForce(direction * force);
        animator.SetTrigger("TakeHit");

        lastAttackType = attackType;
        switch (attackType)
        {
            case PlayerBehaviour.PlayerAttackType.Push:
                stunnedTimer += pushStunTimer;
                FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.EnemyTakePushHit, transform.position);
                break;
            case PlayerBehaviour.PlayerAttackType.Pull:
                isShielded = false;
                shieldObject.SetActive(false);
                FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.EnemyTakeDashHit, transform.position);
                break;
            case PlayerBehaviour.PlayerAttackType.Dash:
                stunnedTimer += dashStunTimer;
                FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.EnemyTakeDashHit, transform.position);
                break;
            case PlayerBehaviour.PlayerAttackType.Heavy:
                stunnedTimer += heavyStunTimer;
                FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.EnemyTakePushHit, transform.position);
                break;
            case PlayerBehaviour.PlayerAttackType.ParryBullet:
                stunnedTimer += pushStunTimer;
                FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.EnemyTakePushHit, transform.position);
                break;
        }

        movementStatus = MovementType.Pushed;
    }
}
