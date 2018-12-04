using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGateTrigger : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "GateCube")
        {
            Animator animator = other.transform.parent.GetComponent<Animator>();
            if (animator != null)
                animator.SetBool("ShouldSlideDown", true);
            else
                Debug.LogError("Could not find animator on GateCube");
        }
    }
}
