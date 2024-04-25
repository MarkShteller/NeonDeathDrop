﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGateTrigger : MonoBehaviour
{
    public PlayerBehaviour player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "GateCube")
        {
            GateTileBehaviour tile = other.transform.GetComponent<GateTileBehaviour>();
            if (tile != null)
                if (player.enemyDefeatedCount >= tile.gateEnemyDeathGoal)
                {
                    if (!tile.isOpen)
                    {
                        tile.SlideDown();
                        tile.isOpen = true;
                    }
                }
                else
                    Debug.LogFormat("GateCube needs {0} enemies defeated.", tile.gateEnemyDeathGoal);
            else
                Debug.LogError("Could not find BaseTileBehaviour on GateCube");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "GateCube")
        {
            GateTileBehaviour tile = other.transform.GetComponent<GateTileBehaviour>();
            if (tile != null)
                if (player.enemyDefeatedCount >= tile.gateEnemyDeathGoal)
                {
                    if (!tile.isOpen)
                    {
                        tile.SlideDown();
                        tile.isOpen = true;
                    }
                }
                else
                    Debug.LogFormat("GateCube needs {0} enemies defeated.", tile.gateEnemyDeathGoal);
            else
                Debug.LogError("Could not find BaseTileBehaviour on GateCube");
        }
    }
}
