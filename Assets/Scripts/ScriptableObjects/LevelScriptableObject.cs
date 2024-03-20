using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using static Loader;

[CreateAssetMenu(fileName = "Level 0-0", menuName = "Level")]
public class LevelScriptableObject : ScriptableObject
{
    public string displayName;
    public AdditiveScenes additiveScene;
    public AudioManager.LevelMusicTracks levelMusicTrack;
    public bool startMusicWithEvent;
    public Texture2D map;
    public string dialogSheetName;

    public Vector3 PlayerSpawnRotation;

    public EnemySpawner[] Spawners;
    public int[] enemyDefeatedCountGoals;
    public float fieldRotationAngle;
    public string musicName;
    public GameObject backdrop;
    public int[] powerupWeights;

    public bool hasSublevels;
    public Vector2 positionOffset;
    public LevelScriptableObject[] linkedSublevelList;

    public bool isPlayVideoOnStart;
    public VideoClip videoClip;

    public bool startPlayerFalling = false;
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
