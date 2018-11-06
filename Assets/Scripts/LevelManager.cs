using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    public string levelName;

    private LevelData levelData;
    private GridNode[,] levelGridNodes;


    void Awake ()
    {
        TextAsset json = Resources.Load<TextAsset>("Levels/" + levelName);
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
	}

    private void Start()
    {
        TerrainManager.Instance.SetGridAndBuild(levelGridNodes);
    }

}

[Serializable]
public class LevelData
{
    public Dictionary<int, int[]> levelTiles;
}

