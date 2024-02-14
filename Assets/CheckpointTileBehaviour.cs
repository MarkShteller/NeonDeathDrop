using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class CheckpointTileBehaviour : BaseTileBehaviour
{
    public VisualEffect effect;

    private bool playedOnce = false;

    public void SetCheckpoint()
    {
        if (!playedOnce)
        {
            effect.Play();
            playedOnce = true;
        }
    }
}
