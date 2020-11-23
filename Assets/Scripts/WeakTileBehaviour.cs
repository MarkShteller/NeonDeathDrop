using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakTileBehaviour : BaseTileBehaviour
{
    public float timeToFall;

    public void StepOnTile(Action callback)
    {
        StartCoroutine(FallCoroutine(callback));
    }

    private IEnumerator FallCoroutine(Action callback)
    {
        yield return new WaitForSeconds(timeToFall);
        FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.EnvTileCracked, transform.position);

        base.Drop();
        callback();
    }
}
