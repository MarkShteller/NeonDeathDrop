using UnityEngine;
using System.Collections;
using SettlersEngine;
using System;

public class GridNode : IPathNode<GridNode>
{

    public enum TileType { Normal, Wall, Pit, Occupied }

    private TileType type { get; set; }
    private GameObject gameNodeRef;
    public float TimeToNormal { get; set; }

    public GridNode(GameObject go)
    {
        gameNodeRef = go;
        type = TileType.Normal;
    }

    public GameObject GetGameNodeRef()
    { return gameNodeRef; }

    //public bool IsWalkable()
    //{
    //    return type == TileType.Normal;
    //}

    public bool IsWalkable(GridNode inContext)
    {
        return type == TileType.Normal || type == TileType.Occupied;
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