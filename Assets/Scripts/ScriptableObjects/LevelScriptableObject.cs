﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level 0-0", menuName = "Level")]
public class LevelScriptableObject : ScriptableObject
{
    public AudioManager.LevelMusicTracks levelMusicTrack;
    public Texture2D map;
    public EnemySpawner[] Spawners;
    public int[] enemyDefeatedCountGoals;
    public float fieldRotationAngle;
    public string musicName;
    public GameObject backdrop;
    public int[] powerupWeights;

    public bool hasSublevels;
    public Vector2 positionOffset;
    public LevelScriptableObject[] linkedSublevelList;

    public bool includeCompanion = true;
    public bool isVRSpace = false;
    
    public bool showEndLevelReport;

    public int targetScore;
    public float targetScoreRankMod;

    public float targetMaxMultiplier;
    public float targetMaxMultiplierRankMod;

    public float targetTime;
    public float targetTimeRankMod;

    public int targetEnemyCount;
    public float targetEnemyCountRankMod;

    public float targetDamage;
    public float targetDamageRankMod;
}
