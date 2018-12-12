using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PowerUp 0", menuName = "PowerUp")]
public class PowerUpObject : ScriptableObject
{
    public string powerUpName;

    public PowerUpType type;
    public float bonus;
    public float effectTime;
    public int count;

    public float weight;

    public GameObject prefab;

    [HideInInspector]
    public DropRange dropRange;
    
}
public enum PowerUpType { Health, RegenBoost, PushForceBoost, PushRangeBoost, Core }
