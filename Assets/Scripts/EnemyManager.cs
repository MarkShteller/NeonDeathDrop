using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

    public GameObject EnemyPrefab;
    public float CreateEnemyInterval;

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
                Instantiate(EnemyPrefab, new Vector3(8f, 0.65f, 5.75f), Quaternion.identity);
            }
            yield return new WaitForSeconds(t);
        }
    }
}
