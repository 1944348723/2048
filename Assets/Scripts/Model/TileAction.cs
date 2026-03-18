using UnityEngine;

public enum TileActionType { Move, Merge, Spawn };
/*
    Move - from, to
    Merge - from1, from2, to, val
    Spawn - to, val
*/

// get返回的是值拷贝，所以TileAction一旦创建内部状态就无法修改
public readonly struct TileAction
{
    private TileAction(Vector2Int from1, Vector2Int from2, Vector2Int to, int val, TileActionType actionType)
    {
        From1 = from1;
        From2 = from2;
        To = to;
        Val = val;
        ActionType = actionType;
    }

    public Vector2Int From1 { get; }
    public Vector2Int From2 { get; }
    public Vector2Int To { get; }
    public int Val { get; }
    public TileActionType ActionType { get; }

    // 禁止构造函数，只能通过提供的工厂方法创建，避免犯错
    public static TileAction Move(Vector2Int from, Vector2Int to)
    {
        return new TileAction(from, default, to, 0, TileActionType.Move);
    }

    public static TileAction Merge(Vector2Int from1, Vector2Int from2, Vector2Int to, int val)
    {
        return new TileAction(from1, from2, to, val, TileActionType.Merge);
    }

    public static TileAction Spawn(Vector2Int to, int val)
    {
        return new TileAction(default, default, to, val, TileActionType.Spawn);
    }
}