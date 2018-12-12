using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGateTrigger : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "GateCube")
        {
            BaseTileBehaviour anim = other.transform.GetComponent<BaseTileBehaviour>();
            if (anim != null)
                anim.SlideDown();
            else
                Debug.LogError("Could not find BaseTileBehaviour on GateCube");
        }
    }
}
