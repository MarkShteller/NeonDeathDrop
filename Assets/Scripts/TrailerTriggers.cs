using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TrailerTriggers : MonoBehaviour
{
    public Animator animator;
    //public VisualEffect vfx;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            GameManager.Instance.PlayerInstance.animator.SetTrigger("T_deahdrop");
            animator.SetTrigger("move");
            //vfx.gameObject.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.PlayerInstance.animator.SetBool("LedgeWiggle", true);
        }
    }
}
