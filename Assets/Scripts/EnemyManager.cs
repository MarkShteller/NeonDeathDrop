using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

    public GameObject EnemyPrefab;
    public float CreateEnemyInterval;
    public Transform SpawnPoint;

	// Use this for initialization
	void Start () {
        StartCoroutine(CreateEnemyEvery(CreateEnemyInterval));
	}

    IEnumerator CreateEnemyEvery(float t)
    {
        while (true)
        {
            if (TerrainManager.Instance.finishedInit && GameManager.Instance.playerPointPosition != null)
            {
                print("instantiating enemy...");
                Instantiate(EnemyPrefab, SpawnPoint.position, Quaternion.identity);
            }
            yield return new WaitForSeconds(t);
        }
    }
}
