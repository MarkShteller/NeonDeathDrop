using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public LevelScriptableObject[] levelDatas;

    public void Init(int levelIndex)
    {
        StartCoroutine(GenerateLevel(levelDatas[levelIndex]));
    }


    private IEnumerator GenerateLevel(LevelScriptableObject levelData)
    {
        yield return null;
        LevelGenerator.Instance.GenerateLevel(levelData);
        StopCoroutine(GenerateLevel(levelData));
    }

}