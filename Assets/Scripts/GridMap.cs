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
    private int rows;
    private int cols;
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

    public void Rotate90Clockwise()
    {
        int[,] newMap = new int[cols, rows];
        for (int r = 0; r < rows; ++r)
        {
            for (int c = 0; c < cols; ++c)
            {
                newMap[c, rows - 1 - r] = map[r, c];
            }
        }
        map = newMap;
        int temp = rows;
        this.rows = cols;
        this.cols = temp;
        this.RebuildEmptyCells();
    }

    public void Rotate180Clockwise()
    {
        int[,] newMap = new int[rows, cols];
        for (int r = 0; r < rows; ++r)
        {
            for (int c = 0; c < cols; ++c)
            {
                newMap[rows - 1 - r, cols - 1 - c] = map[r, c];
            }
        }
        map = newMap;
        this.RebuildEmptyCells();
    }

    public void Rotate270Clockwise()
    {
        int[,] newMap = new int[cols, rows];
        for (int r = 0; r < rows; ++r)
        {
            for (int c = 0; c < cols; ++c)
            {
                newMap[cols - 1 - c, r] = map[r, c];
            }
        }
        map = newMap;
        int temp = rows;
        rows = cols;
        cols = temp;
        this.RebuildEmptyCells();
    }

    // 旋转的是数组下的坐标，不是常规的笛卡尔坐标系
    // 数组下的(0,0)类似笛卡尔坐标系下的(1, 1)，旋转是会变化的
    public Vector2Int Rotate90Clockwise(Vector2Int coord)
    {
        return new Vector2Int(coord.y, rows - 1 - coord.x);
    }

    public Vector2Int Rotate180Clockwise(Vector2Int coord)
    {
        return new Vector2Int(rows - 1 - coord.x, cols - 1 - coord.y);
    }

    public Vector2Int Rotate270Clockwise(Vector2Int coord)
    {
        return new Vector2Int(cols - 1 - coord.y, coord.x);
    }

    public int[] GetRow(int row)
    {
        int[] res = new int[cols];
        for (int col = 0; col < cols; ++col)
        {
            res[col] = this.map[row, col];
        }
        return res;
    }

    public int GetRowCount()
    {
        return this.rows;
    }

    public int GetColCount()
    {
        return this.cols;
    }

    public void Display()
    {
        for (int r = 0; r < rows; ++r)
        {
            Debug.Log(string.Join(' ', GetRow(r)));
        }
        Debug.Log("------------------------------------");
    }

    private void RebuildEmptyCells()
    {
        this.emptyCells.Clear();
        for (int r = 0; r < rows; ++r)
        {
            for (int c = 0; c < cols; ++c)
            {
                if (map[r, c] == 0)
                {
                    emptyCells.Add(new Vector2Int(r, c));
                }
            }
        }
    }
}