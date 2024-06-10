﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisualsBehaviour : MonoBehaviour 
{
    public PlayerBehaviour PlayerBehaviour;

    void OnTriggerEnter(Collider other)
    {
        //print("In radius: " + other.name);
        if (other.tag == "Enemy" || other.tag == "BossSpiderLeg")
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                //PlayerBehaviour.UseMana(PlayerBehaviour.pushManaCost);

                //print("trigger touched enemy");
                Vector3 dir = transform.position - other.transform.position;
                // We then get the opposite (-Vector3) and normalize it
                dir = -dir.normalized;

                if (PlayerBehaviour.currentAttackType == PlayerBehaviour.PlayerAttackType.Push ||
                    PlayerBehaviour.currentAttackType == PlayerBehaviour.PlayerAttackType.Pull)
                {
                    enemy.ForcePush(dir, PlayerBehaviour.currentPushForce, PlayerBehaviour.currentAttackType);
                }
                if (PlayerBehaviour.isDoingSomersault)
                {
                    GameManager.Instance.DashSlomo(10f);
                    GameManager.Instance.cameraRef.FrameHeavyAttack(other.transform);
                    enemy.ForcePush(dir, PlayerBehaviour.currentPushForce, PlayerBehaviour.PlayerAttackType.Heavy);
                }
                ObjectPooler.Instance.SpawnFromPool("HitEffect", enemy.transform.position, enemy.transform.rotation);

            }
        }
        if (other.tag == "Enviromental" && PlayerBehaviour.currentAttackType != PlayerBehaviour.PlayerAttackType.None)
        {
            print("hitting enviromental");
            FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.EnemyBump, other.transform.position);
            Vector3 dir = transform.position - other.transform.position;
            dir = -dir.normalized;
            other.GetComponent<Rigidbody>().AddForce(dir * PlayerBehaviour.currentPushForce * 0.7f);
            StartCoroutine(DestroyObjectAfter(other.gameObject, 10));
        }
        if (other.tag == "Breakable")
        {
            other.GetComponent<BreakableObject>().Break(PlayerBehaviour.transform);
        }
    }

    private IEnumerator DestroyObjectAfter(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        obj.SetActive(false);
    }
}

