using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

    public static EnemyManager Instance;

    //public GameObject EnemyPrefab;
    //public float CreateEnemyInterval;
    public List<EnemySpawnpointInstance> SpawnPoints;

    [HideInInspector]
    public List<Enemy> activeEnemies;
    private List<Enemy> enemiesToRemove;

    public bool isUpdateEnemies;
    public bool isTrackPlayer;

    private int numEnemiesTrackingPlayer = 0;

    private void Awake()
    {
        Instance = this;
        isUpdateEnemies = true;
        isTrackPlayer = true;
        enemiesToRemove = new List<Enemy>();
        activeEnemies = new List<Enemy>();
        SpawnPoints = new List<EnemySpawnpointInstance>();
    }

    public void Init()
    {
        //print("## inting spawn points count: " + SpawnPoints.Count);

        foreach (EnemySpawnpointInstance spawner in SpawnPoints)
        {
            //print("Inting spawn point " + spawner.spawnPoint.position);
            StartCoroutine(SpawnCoroutine(spawner));
        }
    }

    IEnumerator SpawnCoroutine(EnemySpawnpointInstance spawner)
    {
        int quantity = spawner.quantity;
        while (quantity > 0 || quantity <= -1)
        {
            //print("spawner "+spawner.name+" spawning enemy... quantity: "+quantity);
            //Instantiate(spawner.enemy, spawner.spawnPoint.position, Quaternion.identity);
            Enemy enemy = ObjectPooler.Instance.SpawnFromPool(spawner.enemyName, spawner.spawnPoint.position, Quaternion.identity).GetComponent<Enemy>();
            activeEnemies.Add(enemy);

            quantity--;
            yield return new WaitForSeconds(spawner.interval);
        }
    }

    private void FixedUpdate()
    {
        if (isUpdateEnemies)
        {
            if (enemiesToRemove.Count > 0)
            {
                foreach (Enemy e in enemiesToRemove)
                    activeEnemies.Remove(e);
                enemiesToRemove.Clear();
            }
            numEnemiesTrackingPlayer = 0;
            foreach (Enemy enemy in activeEnemies)
            {
                enemy.UpdateEnemy(isTrackPlayer);
                numEnemiesTrackingPlayer += enemy.GetIsTrackingPlayer();
            }
            if(numEnemiesTrackingPlayer > 0)
                AudioManager.Instance.SetHighIntensityMusic();
            else
                AudioManager.Instance.SetLowIntensityMusic();
        }
    }

    public void SetUpdateEnemies(bool b)
    {
        isUpdateEnemies = b;
    }

    public void AddSpawnPoint(EnemySpawnpointInstance spawner)
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

    public Enemy GetStunnedEnemy()
    {
        Enemy e = null;
        foreach (Enemy enemy in activeEnemies)
        {
            if (enemy.isSuperStunned)
                return enemy;
        }
        return e;
    }

    public List<Enemy> GetStunnedEnemiesByDistance(float radius, Vector3 playerPosition)
    {
        List<Enemy> enemies = new List<Enemy>();
        foreach (Enemy enemy in activeEnemies)
        {
            if (enemy.isSuperStunned && Vector3.Distance(playerPosition, enemy.transform.position) <= radius)
                enemies.Add(enemy);
        }
        return enemies;
    }

    public List<Enemy> GetLaunchedEnemies()
    {
        List<Enemy> launchedEnemies = new List<Enemy>();
        foreach (Enemy enemy in activeEnemies)
        {
            if (enemy.movementStatus == Enemy.MovementType.Launched)
                launchedEnemies.Add(enemy);
        }
        return launchedEnemies;
    }

    internal IEnumerator SendLaunchedEnemiesIntoHole(Vector3 holePos, float timeInterval = 1f)
    {
        isTrackPlayer = false;
        List<Enemy> launchedEnemies = GetLaunchedEnemies();
        foreach (Enemy e in launchedEnemies)
        {
            e.FlyToHole(holePos);
            yield return new WaitForSeconds(timeInterval / launchedEnemies.Count);
        }
        yield return new WaitForSeconds(timeInterval);

        isTrackPlayer = true;
    }
}
