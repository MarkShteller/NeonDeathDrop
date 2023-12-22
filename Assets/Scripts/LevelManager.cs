using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public LevelGenerator LevelGeneratorRef;
    public LevelScriptableObject[] levelDatas;

    private List<LevelGenerator> sublevelList;
    private LevelGenerator currentSublevel;
    private int sublevelIndex;


    public LevelScriptableObject Init(int levelIndex)
    {
        StartCoroutine(GenerateLevel(levelDatas[levelIndex], levelIndex));
        return levelDatas[levelIndex];
    }

    public LevelScriptableObject GetLevelData(int levelIndex)
    {
        return levelDatas[levelIndex];
    }

    private IEnumerator GenerateLevel(LevelScriptableObject levelData, int index)
    {
        LevelGenerator levelGenerator = Instantiate(LevelGeneratorRef, this.transform);
        currentSublevel = levelGenerator;
        
        sublevelList = new List<LevelGenerator>();
        sublevelList.Add(currentSublevel);
        sublevelIndex = 0;

        levelGenerator.isVRSpace = levelData.isVRSpace;
        levelGenerator.GenerateBaseLevel(levelData);

        if (levelData.hasSublevels)
        {
            int sublevelIndex = 1;
            float sublevelHeight = -30;
            foreach (var sublevel in levelData.linkedSublevelList)
            {
                LevelGenerator subLevelGenerator = Instantiate(LevelGeneratorRef, this.transform);
                subLevelGenerator.isVRSpace = levelData.isVRSpace;
                subLevelGenerator.sublevelHeight = sublevelHeight;
                subLevelGenerator.GenerateSublevels(sublevel, sublevelIndex);
                sublevelIndex++;
                sublevelHeight -= 30;

                sublevelList.Add(subLevelGenerator);
                yield return null;
            }
        }

        //Start spawning enemies
        StartCoroutine(EnemyManager.Instance.InitAllSpawnpoints());
        //EnemyManager.Instance.SetSublevelActiveEnemies(0);

        PowerupFactory.Instance.SetWeights(levelData.powerupWeights);
        yield return null;
    }

    internal LevelGenerator GetCurrentSublevel()
    {
        return currentSublevel;
    }

    internal LevelGenerator NextSublevel(bool isDown)
    {
        if (isDown)
            sublevelIndex++;
        else
            sublevelIndex--;
        if (sublevelIndex <= sublevelList.Count && sublevelIndex >= 0)
        {
            currentSublevel = sublevelList[sublevelIndex];
            currentSublevel.SetCurrentSublevel();
            EnemyManager.Instance.SetSublevelActiveEnemies(sublevelIndex);
        }
        else
        {
            Debug.LogWarning("Trying to set a sublevel index that's out of bounds");
        }
        return currentSublevel;
    }

    public void ClearActiveLevels()
    {
        foreach (var sublevel in sublevelList)
        {
            Destroy(sublevel.gameObject);
        }
        sublevelList.Clear();
    }
}