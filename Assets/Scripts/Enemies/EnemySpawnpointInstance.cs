using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnpointInstance
{
    public float interval;
    public int quantity;
    public float startTime;
    public string enemyName;
    public Enemy enemy;

    public Transform spawnPoint;

    public EnemySpawnpointInstance(EnemySpawner enemySpawner)
    {
        interval = enemySpawner.interval;
        quantity = enemySpawner.quantity;
        startTime = enemySpawner.startTime;
        enemy = enemySpawner.enemy;
        enemyName = enemySpawner.enemyName;
    }

}
