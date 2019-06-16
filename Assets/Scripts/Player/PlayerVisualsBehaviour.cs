﻿using System.Collections;
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
                //print("trigger touched enemy");
                Vector3 dir = transform.position - other.transform.position;
                // We then get the opposite (-Vector3) and normalize it
                dir = -dir.normalized;
                enemy.ForcePush(dir, PlayerBehaviour.pushForce);

                ObjectPooler.Instance.SpawnFromPool("HitEffect", enemy.transform.position, enemy.transform.rotation);

                /*RaycastHit hit;
                if (Physics.Raycast(transform.parent.position, -transform.forward, out hit))
                {
                    print("enemy pos: " + enemy.transform.position +" hit point: "+hit.point);
                    ObjectPooler.Instance.SpawnFromPool("HitEffect", hit.point, Quaternion.identity);
                }*/

            }
        }
    }
}

