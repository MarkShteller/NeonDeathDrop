using System.Collections.Generic;
using UnityEngine;


public class TerrainManager : MonoBehaviour {

    public static TerrainManager Instance;

    public GameObject cube;

    private GridNode[,] grid;
    private float cubeDistance = 0.1f;

    public bool finishedInit = false;

    private List<GridNode> regeneratingTiles;

    void Awake()
    {
        Instance = this;
        regeneratingTiles = new List<GridNode>();
	}

    void Build()
    {
        Instance = this;
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                switch (grid[i, j].GetTileType())
                {
                    case TileType.Normal:
                        grid[i, j].SetGameNodeRef(CreateTile(i, j, 0.5f));
                        break;
                    case TileType.Wall:
                        grid[i, j].SetGameNodeRef(CreateTile(i, j, 0.4f));
                        break;
                    case TileType.Pit:
                        grid[i, j].SetGameNodeRef(CreatePit(i, j));
                        break;
                    case TileType.PlayerOrigin:
                        GameObject go = CreateTile(i, j, 0.5f);
                        grid[i, j].SetGameNodeRef(go);
                        Vector3 pos = go.transform.localPosition;
                        //GameManager.Instance.SetPlayerPosition(new Vector3(pos.x, 0.65f, pos.z));
                        break;
                }
            }
        }
        finishedInit = true;

        transform.rotation = Quaternion.Euler(0, 45, 0);
    }


    private GameObject CreateTile(int i, int j, float height)
    {
        Vector3 position = new Vector3(i * cube.transform.localScale.x + i * cubeDistance, -cube.transform.localScale.y * height, j * cube.transform.localScale.z + j * cubeDistance);
        Quaternion rotation = Quaternion.Euler(0, 0, 0);
        GameObject go = Instantiate(cube, position, rotation, this.transform) as GameObject;
        go.name = i + "," + j;
        return go;
    }

    private GameObject CreatePit(int i, int j)
    {
        GameObject go = CreateTile(i, j, 3f);
        return go;
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
                tile.SetType(TileType.Normal);
                //set back to original height
                tile.GetGameNodeRef().transform.position = new Vector3(tile.GetGameNodeRef().transform.position.x, -cube.transform.localScale.y / 2, tile.GetGameNodeRef().transform.position.z);
            }
        }
        if(tileToRemove != null)
            regeneratingTiles.Remove(tileToRemove);
    }

    public void SetGridNodeType(int x, int y, TileType type, float regenTime = 0)
    {
        grid[x, y].SetType(type);
        if (type == TileType.Pit)
        {
            regeneratingTiles.Add(grid[x, y]);
            grid[x, y].TimeToNormal = regenTime;
        }
    }

    public TileType GetGridNodeType(int x, int y)
    {
        return grid[x, y].GetTileType();
    }

    public void SetGridAndBuild(GridNode[,] gridNodes)
    {
        grid = gridNodes;
        Build();
    }

    public GridNode[,] GetGrid()
    {
        if (finishedInit)
            return grid;
        else
        {
            Debug.LogError("TRYING TO ACCESS TERRAIN BEFORE FINISHED INIT.");
            return null;
        }
    }
	
}
