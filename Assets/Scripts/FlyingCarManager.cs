using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingCarManager : MonoBehaviour
{
    private ObjectPooler objectPooler;
    public float carSpawnRate;
    public CustomDollyCart[] customDollies;

    void Start()
    {
        objectPooler = ObjectPooler.Instance;
        StartCoroutine(SpawnCarEvery(carSpawnRate));
    }

    private IEnumerator SpawnCarEvery(float time)
    {
        while (true)
        {
            GameObject go = objectPooler.SpawnFromPool("Car1", Vector3.zero, Quaternion.identity);
            customDollies[0].AddCart(go.transform);
            yield return new WaitForSeconds(time);
        }
    }

}
