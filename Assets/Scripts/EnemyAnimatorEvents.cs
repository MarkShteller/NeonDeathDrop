using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorEvents : MonoBehaviour
{
    public Enemy enemy;

    public void DeathEvent()
    {
        enemy.DeathEvent();
    }

    public void FireEvent()
    {
        enemy.FireEvent();
    }

    public void PulseEvent()
    {
        enemy.PulseTrigger();
    }
}
