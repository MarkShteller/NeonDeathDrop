using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

    public static EnemyManager Instance;

    //public GameObject EnemyPrefab;
    //public float CreateEnemyInterval;
    public List<EnemySpawner> SpawnPoints;

    private void Awake()
    {
        Instance = this;
    }

    IEnumerator SpawnCoroutine(EnemySpawner spawner)
    {
        int quantity = spawner.quantity;
        while (quantity > 0 || quantity <= -1)
        {
            print("spawner "+spawner.name+" spawning enemy... quantity: "+quantity);
            //Instantiate(spawner.enemy, spawner.spawnPoint.position, Quaternion.identity);
            ObjectPooler.Instance.SpawnFromPool(spawner.enemyName, spawner.spawnPoint.position, Quaternion.identity);
            quantity--;
            yield return new WaitForSeconds(spawner.interval);
        }
    }

    /*IEnumerator CreateEnemyEvery(float t)
    {
        while (true)
        {
            if (SpawnPoints.Count > 0 && LevelGenerator.Instance.finishedInit && GameManager.Instance.playerPointPosition != null)
            {
                int rndPos = Random.Range(0, SpawnPoints.Count);
                print("Instantiating enemy on point index: "+ rndPos);
                Instantiate(EnemyPrefab, SpawnPoints[rndPos].position, Quaternion.identity);
            }
            yield return new WaitForSeconds(t);
        }
    }*/

    public void AddSpawnPoint(EnemySpawner spawner)
    {
        SpawnPoints.Add(spawner);
    }

    public void Init()
    {
        print("## inting spawn points count: " + SpawnPoints.Count);

        foreach (EnemySpawner spawner in SpawnPoints)
        {
            print("Inting spawn point "+ spawner.spawnPoint.position);
            StartCoroutine(SpawnCoroutine(spawner));
        }
    }

    public void SpawnEnemiesForBossBattle(BossEnemySpawnPoint[] enemySpawners)
    {
        foreach (BossEnemySpawnPoint spawner in enemySpawners)
        {
            ObjectPooler.Instance.SpawnFromPool(spawner.enemySpawner.enemyName, spawner.transform.position, Quaternion.identity);
        }
    }
}
