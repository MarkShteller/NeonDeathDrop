using UnityEngine;
using System.Collections;
using SettlersEngine;
using System;


public enum TileType { Pit, Normal, Wall, WallHigh, Occupied, PlayerPit, EnemyPit, PlayerOrigin = 10, EnemySpawn, Goal, Gate, Checkpoint, Weak, Immoveable, SublevelDown, SublevelUp, None = 99}

public class GridNode : IPathNode<GridNode>
{
    private TileType type { get; set; }
    private GameObject gameNodeRef;
    public float TimeToNormal { get; set; }

    public GridNode(GameObject go)
    {
        gameNodeRef = go;
        type = TileType.Normal;
    }

    public GridNode(int tileId)
    {
        type = (TileType) tileId;
    }

    public GridNode(TileType tileType)
    {
        type = tileType;
    }

    public GameObject GetGameNodeRef()
    { return gameNodeRef; }

    public void SetGameNodeRef(GameObject go)
    {
        gameNodeRef = go;
    }

    //public bool IsWalkable()
    //{
    //    return type == TileType.Normal;
    //}

    public bool IsWalkable(GridNode inContext)
    {
        return type == TileType.Normal || type == TileType.Occupied || type == TileType.Weak || type == TileType.Immoveable;
    }

    public void SetType(TileType type)
    {
        this.type = type;
    }

    public TileType GetTileType()
    {
        return type;
    }
}

public class Point
{
    public int x;
    public int y;

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}