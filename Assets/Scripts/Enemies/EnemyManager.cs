using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

    public static EnemyManager Instance;

    public GameObject EnemyPrefab;
    public float CreateEnemyInterval;
    public List<Transform> SpawnPoints;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start () {
        StartCoroutine(CreateEnemyEvery(CreateEnemyInterval));
	}

    IEnumerator CreateEnemyEvery(float t)
    {
        while (true)
        {
            if (SpawnPoints.Count > 0 && TerrainManager.Instance.finishedInit && GameManager.Instance.playerPointPosition != null)
            {
                int rndPos = Random.Range(0, SpawnPoints.Count);
                print("Instantiating enemy on point index: "+ rndPos);
                Instantiate(EnemyPrefab, SpawnPoints[rndPos].position, Quaternion.identity);
            }
            yield return new WaitForSeconds(t);
        }
    }

    public void AddSpawnPoint(Transform spawnTransform)
    {
        SpawnPoints.Add(spawnTransform);
    }
}
