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
}
