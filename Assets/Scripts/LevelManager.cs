using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public LevelScriptableObject[] levelDatas;

    private int prevLevelIndex = 0;

    public void Init(int levelIndex)
    {
        StartCoroutine(GenerateLevel(levelDatas[levelIndex], levelIndex));
    }


    private IEnumerator GenerateLevel(LevelScriptableObject levelData, int index)
    {
        yield return null;
        LevelGenerator.Instance.GenerateLevel(levelData);
        //StopCoroutine(GenerateLevel(levelData));

        if (!levelData.musicName.Equals(levelDatas[prevLevelIndex].musicName))
        {
            AudioManager.Instance.LoadMusicToSecondarySource(levelData.musicName);
            AudioManager.Instance.CrossfadeMusic(0.01f);
        }
        prevLevelIndex = index;
    }

}