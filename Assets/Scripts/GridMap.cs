using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Direction { Up, Down, Left, Right };

public static class DirectionExtensions {
    public static Vector2Int ToVector(this Direction dir) {
        switch (dir) {
            case Direction.Up:
                return new Vector2Int(-1, 0);
            case Direction.Down:
                return new Vector2Int(1, 0);
            case Direction.Left:
                return new Vector2Int(0, -1);
            case Direction.Right:
                return new Vector2Int(0, 1);
            default:
                return new Vector2Int(0, 0);
        }
    }
}

public class GridMap
{
    private int[,] map;
    private int rows = 10;
    private int cols = 10;
    private Vector2 cellSize = new Vector2(1, 1);
    private Vector2 gap = new Vector2(0, 0);
    private Vector2 leftTop;
    private HashSet<Vector2Int> emptyCells = new HashSet<Vector2Int>();

    public GridMap(int rows, int cols)
    {
        this.rows = rows;
        this.cols = cols;
        this.map = new int[rows, cols];
        for (int r = 0; r < rows; ++r)
        {
            for (int c = 0; c < cols; ++c)
            {
                map[r, c] = 0;
                emptyCells.Add(new Vector2Int(r, c));
            }
        }
    }

    // 需要映射到世界坐标系时调用
    public void Init(Vector2 cellSize, Vector2 gap, Vector2 centerPos)
    {
        this.cellSize = cellSize;
        this.gap = gap;
        // (0, 0)元素中心的世界坐标
        this.leftTop = new Vector2(
            (-cols / 2.0f + 0.5f) * cellSize.x - (cols - 1) / 2f * gap.x,
            (rows / 2.0f - 0.5f) * cellSize.y + (rows - 1) / 2f * gap.y
        ) + centerPos;
    }

    public Vector2 GridToWorld(int row, int col)
    {
        return new Vector2(this.leftTop.x + col * cellSize.x + col * gap.x, this.leftTop.y - row * cellSize.y - row * gap.y);
    }

    public int Get(int row, int col)
    {
        return map[row, col];
    }

    public void Set(int row, int col, int val)
    {
        if (map[row, col] == 0 && val != 0) {
            emptyCells.Remove(new Vector2Int(row, col));
        } else if (map[row, col] != 0 && val == 0) {
            emptyCells.Add(new Vector2Int(row, col));
        }
        map[row, col] = val;
    }

    // O(n)
    public Vector2Int GetRandomEmptyCoordinate() {
        return emptyCells.ElementAt(Random.Range(0, emptyCells.Count));
    }

    public void Fill(int val)
    {
        for (int r = 0; r < rows; ++r)
        {
            for (int c = 0; c < cols; ++c)
            {
                Set(r, c, val);
            }
        }
    }
}