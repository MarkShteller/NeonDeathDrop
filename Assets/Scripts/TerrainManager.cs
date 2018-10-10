using System.Collections.Generic;
using UnityEngine;


public class TerrainManager : MonoBehaviour {

    public static TerrainManager Instance;

    public GameObject cube;
    public GameObject barrierCube;
    private GridNode[,] grid;
    private int gridSizeX = 50;
    private int gridSizeY = 10;
    private float cubeDistance = 0.1f;

    public bool finishedInit = false;

    private List<GridNode> regeneratingTiles;

    // Use this for initialization
    void Awake()//Start ()
    {
        Instance = this;
        grid = new GridNode[gridSizeX, gridSizeY];
        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeY; j++)
            {
                Vector3 position = new Vector3(i * cube.transform.localScale.x + i*cubeDistance, -cube.transform.localScale.y / 2, j * cube.transform.localScale.z + j*cubeDistance);
                Quaternion rotation = Quaternion.Euler(0, 0, 0);
                GameObject go = Instantiate(cube, position, rotation, this.transform) as GameObject;
                go.name = i + "," + j;

                grid[i, j] = new GridNode(go);
            }
        }
        finishedInit = true;
        regeneratingTiles = new List<GridNode>();
	}

    void Update()
    {
        GridNode tileToRemove = null;
        foreach (GridNode tile in regeneratingTiles)
        {
            tile.TimeToNormal -= Time.deltaTime;
            if (tile.TimeToNormal <= 0)
            {
                tileToRemove = tile;
                tile.SetType(GridNode.TileType.Normal);
                //set back to original height
                tile.GetGameNodeRef().transform.position = new Vector3(tile.GetGameNodeRef().transform.position.x, -cube.transform.localScale.y / 2, tile.GetGameNodeRef().transform.position.z);
            }
        }
        if(tileToRemove != null)
            regeneratingTiles.Remove(tileToRemove);
    }

    public void SetGridNodeType(int x, int y, GridNode.TileType type, float regenTime = 0)
    {
        grid[x, y].SetType(type);
        if (type == GridNode.TileType.Pit)
        {
            regeneratingTiles.Add(grid[x, y]);
            grid[x, y].TimeToNormal = regenTime;

            //place a collider on the pit so the player would not fall in
            //todo
        }
    }

    public GridNode.TileType GetGridNodeType(int x, int y)
    {
        return grid[x, y].GetTileType();
    }

    public GridNode[,] GetGrid()
    {
        if (finishedInit)
            return grid;
        else
        {
            Debug.LogError("TRYING TO ACCESS TERRAIN BEFORE IT FINISHED INIT.");
            return null;
        }
    }
	
}
