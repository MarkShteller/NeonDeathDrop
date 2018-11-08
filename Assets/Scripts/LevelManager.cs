using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public TextAsset[] levelNames;

    private LevelData levelData;
    private GridNode[,] levelGridNodes;

    public void Init (int levelIndex)
    {
        TextAsset json = levelNames[levelIndex];
        LevelData levelData = JsonConvert.DeserializeObject<LevelData>(json.text);

        levelGridNodes = new GridNode[levelData.levelTiles.Count, levelData.levelTiles[0].Length];
        foreach (var row in levelData.levelTiles)
        {
            int[] rowValues = row.Value;
            for (int i = 0; i < rowValues.Length; i++)
            {
                levelGridNodes[row.Key, i] = new GridNode(rowValues[i]);
            }
        }

        StartCoroutine(BuildLevel());
    }

    private IEnumerator BuildLevel()
    {
        yield return null;
        TerrainManager.Instance.SetGridAndBuild(levelGridNodes);
    }
}

[System.Serializable]
public class LevelData
{
    public Dictionary<int, int[]> levelTiles;
}

