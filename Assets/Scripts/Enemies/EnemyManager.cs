using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

    public static EnemyManager Instance;

    //public GameObject EnemyPrefab;
    //public float CreateEnemyInterval;
    public List<EnemySpawner> SpawnPoints;

    [HideInInspector]
    public List<Enemy> activeEnemies;
    private List<Enemy> enemiesToRemove;

    public bool isUpdateEnemies;

    private void Awake()
    {
        Instance = this;
        isUpdateEnemies = true;
        enemiesToRemove = new List<Enemy>();
        activeEnemies = new List<Enemy>();
    }

    public void Init()
    {
        print("## inting spawn points count: " + SpawnPoints.Count);

        foreach (EnemySpawner spawner in SpawnPoints)
        {
            print("Inting spawn point " + spawner.spawnPoint.position);
            StartCoroutine(SpawnCoroutine(spawner));
        }
    }

    IEnumerator SpawnCoroutine(EnemySpawner spawner)
    {
        int quantity = spawner.quantity;
        while (quantity > 0 || quantity <= -1)
        {
            print("spawner "+spawner.name+" spawning enemy... quantity: "+quantity);
            //Instantiate(spawner.enemy, spawner.spawnPoint.position, Quaternion.identity);
            Enemy enemy = ObjectPooler.Instance.SpawnFromPool(spawner.enemyName, spawner.spawnPoint.position, Quaternion.identity).GetComponent<Enemy>();
            activeEnemies.Add(enemy);

            quantity--;
            yield return new WaitForSeconds(spawner.interval);
        }
    }

    private void Update()
    {
        if (isUpdateEnemies)
        {
            if (enemiesToRemove.Count > 0)
            {
                foreach (Enemy e in enemiesToRemove)
                    activeEnemies.Remove(e);
                enemiesToRemove.Clear();
            }
            foreach (Enemy enemy in activeEnemies)
            {
                enemy.UpdateEnemy();
            }
        }
    }

    public void AddSpawnPoint(EnemySpawner spawner)
    {
        SpawnPoints.Add(spawner);
    }

    public void SpawnEnemiesForBossBattle(BossEnemySpawnPoint[] enemySpawners)
    {
        foreach (BossEnemySpawnPoint spawner in enemySpawners)
        {
            Enemy enemy = ObjectPooler.Instance.SpawnFromPool(spawner.enemySpawner.enemyName, spawner.transform.position, Quaternion.identity).GetComponent<Enemy>();
            activeEnemies.Add(enemy);
        }
    }

    internal void RemoveFromActiveEnemies(Enemy enemy)
    {
        enemiesToRemove.Add(enemy);
    }
}
