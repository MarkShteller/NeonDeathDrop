using System.Collections.Generic;
using UnityEngine;


public class TerrainManager : MonoBehaviour {

    public static TerrainManager Instance;

    public GameObject cube;
    public GameObject cubeGoal;

    private GridNode[,] grid;
    private float cubeDistance = 0.1f;

    public bool finishedInit = false;

    private List<GridNode> regeneratingTiles;

    private const float NORMAL_TILE_HEIGHT = 0.5f;
    private const float WALL_TILE_HEIGHT = 0.4f;
    private const float PIT_TILE_HEIGHT = 3f;
    private const float CHARACTER_POS_HEIGHT = 0.65f;

    void Awake()
    {
        Instance = this;
        regeneratingTiles = new List<GridNode>();
	}

    void Build()
    {
        Instance = this;
        GameObject PlayerSpawnObj = null;
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                switch (grid[i, j].GetTileType())
                {
                    case TileType.Normal:
                        grid[i, j].SetGameNodeRef(CreateTile(i, j, NORMAL_TILE_HEIGHT, cube));
                        break;
                    case TileType.Wall:
                        grid[i, j].SetGameNodeRef(CreateTile(i, j, WALL_TILE_HEIGHT, cube));
                        break;
                    case TileType.Pit:
                        grid[i, j].SetGameNodeRef(CreatePit(i, j));
                        break;
                    case TileType.PlayerOrigin:
                        GameObject goo = CreateTile(i, j, NORMAL_TILE_HEIGHT, cube);
                        grid[i, j].SetGameNodeRef(goo);
                        PlayerSpawnObj = CreateSpawnPoint("PlayerSpawn", goo.transform.position);
                        break;
                    case TileType.EnemySpawn:
                        {
                            GameObject go = CreateTile(i, j, NORMAL_TILE_HEIGHT, cube);
                            grid[i, j].SetGameNodeRef(go);
                            Vector3 pos = go.transform.position;
                            EnemyManager.Instance.AddSpawnPoint(CreateSpawnPoint("EmemySpawn", pos).transform);
                        }
                        break;
                    case TileType.Goal:
                        grid[i, j].SetGameNodeRef(CreateTile(i, j, NORMAL_TILE_HEIGHT, cubeGoal));
                        break;
                }
            }
        }
        finishedInit = true;

        transform.rotation = Quaternion.Euler(0, 45, 0);

        if (PlayerSpawnObj != null)
            GameManager.Instance.SetPlayerPosition(PlayerSpawnObj.transform.position);
        else
            Debug.LogWarning("Trying to create a level without a player origin!");
    }

    private GameObject CreateSpawnPoint(string name, Vector3 pos)
    {
        GameObject spawnObj = Instantiate(new GameObject(name),
                                                                new Vector3(pos.x, CHARACTER_POS_HEIGHT, pos.z),
                                                                Quaternion.identity,
                                                                transform);
        return spawnObj;
    }

    private GameObject CreateTile(int i, int j, float height, GameObject origin)
    {
        Vector3 position = new Vector3(i * origin.transform.localScale.x + i * cubeDistance,
                                         -origin.transform.localScale.y * height,
                                         j * origin.transform.localScale.z + j * cubeDistance);
        Quaternion rotation = Quaternion.Euler(0, 0, 0);
        GameObject go = Instantiate(origin, position, rotation, this.transform) as GameObject;
        go.name = i + "," + j;
        return go;
    }

    private GameObject CreatePit(int i, int j)
    {
        GameObject go = CreateTile(i, j, PIT_TILE_HEIGHT, cube);
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
