using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseEffectBehavoir : MonoBehaviour
{
    public SphereCollider sCollider;

    private void OnEnable()
    {
        StartCoroutine(DisableColliderIn(0.5f));
    }

    IEnumerator DisableColliderIn(float sec)
    {
        sCollider.enabled = true;
        yield return new WaitForSeconds(sec);
        sCollider.enabled = false;
    }
}
