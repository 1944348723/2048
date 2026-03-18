using UnityEngine;

public enum Direction { Up, Down, Left, Right };

public static class DirectionExtensions {
    public static Vector2Int ToVector(this Direction dir) {
        return dir switch
        {
            Direction.Up => new Vector2Int(-1, 0),
            Direction.Down => new Vector2Int(1, 0),
            Direction.Left => new Vector2Int(0, -1),
            Direction.Right => new Vector2Int(0, 1),
            _ => new Vector2Int(0, 0),
        };
    }
}