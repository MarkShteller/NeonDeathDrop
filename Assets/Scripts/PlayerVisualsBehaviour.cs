using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisualsBehaviour : MonoBehaviour 
{
    public PlayerBehaviour PlayerBehaviour;

    void OnTriggerEnter(Collider other)
    {
        //print("In radius: " + other.name);
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            print("trigger touchen enemy");
            Vector3 dir = transform.position - other.transform.position;
            // We then get the opposite (-Vector3) and normalize it
            dir = -dir.normalized;
            enemy.ForcePush(dir, PlayerBehaviour.pushForce);
        }
        else print("Collition did not contain enemy.");
    }
}
