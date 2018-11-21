using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpawner 0", menuName = "Enemy Spawner")]
public class EnemySpawner : ScriptableObject
{
    public float interval;
    public int quantity;
    public float startTime;
    public string enemyName;
    public Enemy enemy;

    public Transform spawnPoint;

    public EnemySpawner(EnemySpawner enemySpawner)
    {
        interval = enemySpawner.interval;
        quantity = enemySpawner.quantity;
        startTime = enemySpawner.startTime;
        enemy = enemySpawner.enemy;
        spawnPoint = enemySpawner.spawnPoint;
        enemyName = enemySpawner.enemyName;
    }
}
