using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisualsBehaviour : MonoBehaviour 
{
    public PlayerBehaviour PlayerBehaviour;

    void OnTriggerEnter(Collider other)
    {
        //print("In radius: " + other.name);
        if (other.tag == "Enemy")
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                //PlayerBehaviour.UseMana(PlayerBehaviour.pushManaCost);

                //print("trigger touched enemy");
                Vector3 dir = transform.position - other.transform.position;
                // We then get the opposite (-Vector3) and normalize it
                dir = -dir.normalized;
                enemy.ForcePush(dir, PlayerBehaviour.currentPushForce, PlayerBehaviour.PlayerAttackType.Push);
                if (PlayerBehaviour.isDoingSomersault)
                {
                    GameManager.Instance.DashSlomo(10f);
                    enemy.ForcePush(dir, PlayerBehaviour.currentPushForce, PlayerBehaviour.PlayerAttackType.Heavy);
                }

                ObjectPooler.Instance.SpawnFromPool("HitEffect", enemy.transform.position, enemy.transform.rotation);
            }
        }
        if (other.tag == "Breakable")
        {
            other.GetComponent<BreakableObject>().Break(PlayerBehaviour.transform);
        }
    }
}

