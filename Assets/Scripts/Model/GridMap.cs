using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridMap
{
    private int[,] map;
    private int rows;
    private int cols;

    // 值为0时表示空格子
    private readonly HashSet<Vector2Int> emptyCells = new();

    public GridMap(int rows, int cols)
    {
        if (rows < 1) throw new ArgumentOutOfRangeException(nameof(rows), "rows must be greater than 0.");
        if (cols < 1) throw new ArgumentOutOfRangeException(nameof(cols), "cols must be greater than 0.");
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

    public int Get(int row, int col)
    {
        CheckBounds(row, col);
        return map[row, col];
    }

    public void Set(int row, int col, int val)
    {
        CheckBounds(row, col);
        if (map[row, col] == 0 && val != 0) {
            emptyCells.Remove(new Vector2Int(row, col));
        } else if (map[row, col] != 0 && val == 0) {
            emptyCells.Add(new Vector2Int(row, col));
        }
        map[row, col] = val;
    }

    // O(n)
    public bool TryGetRandomEmptyCoordinate(out Vector2Int coordinate)
    {
        if (emptyCells.Count == 0)
        {
            coordinate = default;
            return false;
        }

        coordinate = emptyCells.ElementAt(UnityEngine.Random.Range(0, emptyCells.Count));
        return true;
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
        Swap(ref rows, ref cols);
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
        Swap(ref rows, ref cols);
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

    // 复制一份
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

    private void CheckBounds(int row, int col)
    {
        if (row < 0 || row >= rows || col < 0 || col >= cols)
        {
            throw new IndexOutOfRangeException("Row or column is out of bounds.");
        }
    }

    private void Swap(ref int a, ref int b)
    {
        (b, a) = (a, b);
    }
}