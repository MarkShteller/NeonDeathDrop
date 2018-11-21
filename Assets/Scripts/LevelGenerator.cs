using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {

    public static LevelGenerator Instance;

	public Texture2D map;
	public ColorToEnum[] colorMappings;


    public GameObject cube;
    public GameObject cubeGoal;

    private LevelScriptableObject levelData;

    private GridNode[,] grid;
    private float cubeDistance = 0.1f;

    public bool finishedInit = false;

    private List<GridNode> regeneratingTiles;

    private GameObject PlayerSpawnObj = null;
    private const float NORMAL_TILE_HEIGHT = 0.5f;
    private const float WALL_TILE_HEIGHT = 0.4f;
    private const float PIT_TILE_HEIGHT = 3f;
    private const float CHARACTER_POS_HEIGHT = 0.65f;

    void Awake()
    {
        Instance = this;
        regeneratingTiles = new List<GridNode>();
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

    internal GridNode[,] GetGridPortion(Point point, int sizeX, int sizeY)
    {
        int startX = point.x - sizeX / 2;
        startX = startX < 0 ? 0 : startX;

        int endX = point.x + sizeX / 2;
        endX = endX > grid.GetLength(0) ? grid.GetLength(0) - 1 : endX;

        int startY = point.y- sizeY / 2;
        startY = startY < 0 ? 0 : startY;

        int endY = point.y + sizeY / 2;
        endY = endY > grid.GetLength(1) ? grid.GetLength(1) - 1 : endY;

        GridNode[,] subGrid = new GridNode[endX-startX, endY-startY];
        for (int i = startX; i < endX; i++)
        {
            for (int j = startY; j < endY; j++)
            {
                subGrid[i-startX, j-startY] = grid[i, j];
            }
        }
        Debug.LogFormat("grid size: [{0},{1}] subgrid size: [{2},{3}] subgrid start: [{4},{5}] subgrid end: [{6},{7}]",
            grid.GetLength(0), grid.GetLength(1), subGrid.GetLength(0), subGrid.GetLength(1), startX, startY, endX, endY);
        return subGrid;
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
        if (tileToRemove != null)
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

	public void GenerateLevel (LevelScriptableObject levelData)
	{
        this.levelData = levelData;
        map = levelData.map;
        PlayerSpawnObj = null;
        grid = new GridNode[map.width, map.height];

        for (int x = 0; x < map.width; x++)
		{
			for (int y = 0; y < map.height; y++)
			{
				GenerateTile(x, y);
			}
		}

        finishedInit = true;

        transform.rotation = Quaternion.Euler(0, levelData.fieldRotationAngle, 0);

        if (PlayerSpawnObj != null)
            GameManager.Instance.SetPlayerPosition(PlayerSpawnObj.transform.position);
        else
            Debug.LogWarning("Trying to create a level without a player origin!");

        EnemyManager.Instance.Init();
    }

	void GenerateTile (int x, int y)
	{
		Color pixelColor = map.GetPixel(x, y);

        if (pixelColor.a == 0)
		{
            // The pixel is transparrent. Let's ignore it!
            grid[x, y] = new GridNode(TileType.None);
            return;
		}

        foreach (ColorToEnum colorMapping in colorMappings)
		{
			if (colorMapping.color.Equals(pixelColor))
			{
                //print("Tile type:" + colorMapping.tileType + " pixel color: " + pixelColor);
                grid[x, y] = new GridNode(colorMapping.tileType);
                switch (colorMapping.tileType)
                {
                    case TileType.Normal:
                        grid[x, y].SetGameNodeRef(CreateTile(x, y, NORMAL_TILE_HEIGHT, cube));
                        break;
                    case TileType.Wall:
                        grid[x, y].SetGameNodeRef(CreateTile(x, y, WALL_TILE_HEIGHT, cube));
                        break;
                    case TileType.Pit:
                        grid[x, y].SetGameNodeRef(CreatePit(x, y));
                        break;
                    case TileType.PlayerOrigin:
                        GameObject goo = CreateTile(x, y, NORMAL_TILE_HEIGHT, cube);
                        grid[x, y].SetGameNodeRef(goo);
                        PlayerSpawnObj = CreateSpawnPoint("PlayerSpawn", goo.transform.position);
                        break;
                    case TileType.EnemySpawn:
                        {
                            /*GameObject go = CreateTile(x, y, NORMAL_TILE_HEIGHT, cube);
                            grid[x, y].SetGameNodeRef(go);
                            Vector3 pos = go.transform.position;
                            EnemyManager.Instance.AddSpawnPoint(CreateSpawnPoint("EnemySpawn", pos).transform);*/
                        }
                        break;
                    case TileType.Goal:
                        grid[x, y].SetGameNodeRef(CreateTile(x, y, NORMAL_TILE_HEIGHT, cubeGoal));
                        break;
                    case TileType.None:
                        grid[x, y] = new GridNode(TileType.None);
                        break;
                }
			}
        }

        if (pixelColor.r > 0.8f && pixelColor.g == 0 && pixelColor.b == 0) // means the pixel is red = enemy
        {
            //The index in the spawner ref array is the RED int value in opposite order (255 = 0, 254 = 1...)
            int spawnerIndex = 255 - Mathf.CeilToInt(pixelColor.r * 255);
            print("spawner index: " +spawnerIndex);
            EnemySpawner enemySpawner = new EnemySpawner(levelData.Spawners[spawnerIndex]);


            GameObject go = CreateTile(x, y, NORMAL_TILE_HEIGHT, cube);
            grid[x, y] = new GridNode(TileType.EnemySpawn);
            grid[x, y].SetGameNodeRef(go);

            Vector3 pos = go.transform.position;
            enemySpawner.spawnPoint = CreateSpawnPoint("EnemySpawn " + spawnerIndex, pos).transform;

            EnemyManager.Instance.AddSpawnPoint(enemySpawner);
        }
    }
	
}
