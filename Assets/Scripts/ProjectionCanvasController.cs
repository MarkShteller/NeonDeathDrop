using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectionCanvasController : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }


    public void SetTrigger(string name)
    {
        animator.SetTrigger(name);
    }
}
