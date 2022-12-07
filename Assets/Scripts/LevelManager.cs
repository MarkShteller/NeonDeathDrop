using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public LevelScriptableObject[] levelDatas;

    private int prevLevelIndex = 0;

    public LevelScriptableObject Init(int levelIndex)
    {
        StartCoroutine(GenerateLevel(levelDatas[levelIndex], levelIndex));
        return levelDatas[levelIndex];
    }


    private IEnumerator GenerateLevel(LevelScriptableObject levelData, int index)
    {
        yield return null;
        LevelGenerator.Instance.GenerateLevel(levelData);
        //StopCoroutine(GenerateLevel(levelData));

        if (!levelData.musicName.Equals(levelDatas[prevLevelIndex].musicName))
        {
            //AudioManager.Instance.LoadMusicToSecondarySource(levelData.musicName);
           // AudioManager.Instance.CrossfadeMusic(0.02f);
        }
        prevLevelIndex = index;

        PowerupFactory.Instance.SetWeights(levelData.powerupWeights);
    }

}