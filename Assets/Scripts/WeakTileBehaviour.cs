using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakTileBehaviour : BaseTileBehaviour
{
    public float timeToFall;
    public GameObject visuals;
    private bool isSteppedOn = false;

    public void StepOnTile(Action callback)
    {
        if (!isSteppedOn)
        { 
            StartCoroutine(FallCoroutine(callback));
            isSteppedOn = true;
        }
    }

    private IEnumerator FallCoroutine(Action callback)
    {
        yield return new WaitForSeconds(timeToFall);
        FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.EnvTileCracked, transform.position);

        StartCoroutine(WeakDrop(5, () => visuals.SetActive(false)));
        callback();
    }
}
