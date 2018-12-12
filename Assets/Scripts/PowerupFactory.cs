using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupFactory : MonoBehaviour {

    public static PowerupFactory Instance;
    public List<PowerUpObject> powerupList;
    public float nullWeight = 40;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        CalcRanges();
        RollPowerup();
    }

    public void CalcRanges()
    {
        float lowTemp = nullWeight;
        foreach (PowerUpObject powerup in powerupList)
        {
            powerup.dropRange.low = lowTemp + 1;
            powerup.dropRange.high = lowTemp + powerup.weight;
            lowTemp = powerup.dropRange.high;
            Debug.LogFormat("{0} powerup range: {1}-{2}", powerup.name, powerup.dropRange.low, powerup.dropRange.high);
        }
    }

    public PowerUpObject RollPowerup()
    {
        int total = (int)(0 + nullWeight);
        powerupList.ForEach(p => total += (int)p.weight);
        int rolledNum = Random.Range(1, total);

        print("Rolled num: " + rolledNum + " / " + total);

        foreach (PowerUpObject p in powerupList)
        {
            if (rolledNum >= p.dropRange.low && rolledNum <= p.dropRange.high)
                return p;
        }

        return null;
    }
}

public struct DropRange
{
    public float low;
    public float high;
}