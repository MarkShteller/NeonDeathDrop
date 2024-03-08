using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateObjectAfter : MonoBehaviour
{
    public GameObject obj;
    public float time;

    void Awake()
    {
        StartCoroutine(ActivateAfter(time));
    }

    private IEnumerator ActivateAfter(float t)
    {
        yield return new WaitForSeconds(t);
        obj.SetActive(true);
    }

}
