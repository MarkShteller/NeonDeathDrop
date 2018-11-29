using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level 0-0", menuName = "Level")]
public class LevelScriptableObject : ScriptableObject
{
    public Texture2D map;
    public EnemySpawner[] Spawners;
    public float fieldRotationAngle;
    public string musicName;

}
