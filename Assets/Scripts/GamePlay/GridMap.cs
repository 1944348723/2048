using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridMap
{
    public int Rows { get; private set; }
    public int Cols { get; private set; }
    private int[,] data;
    // 值为0时表示空格子
    private readonly HashSet<Vector2Int> emptyCells = new();

    public GridMap(int rows, int cols)
    {
        if (rows < 1) throw new ArgumentOutOfRangeException(nameof(rows), "rows must be greater than 0.");
        if (cols < 1) throw new ArgumentOutOfRangeException(nameof(cols), "cols must be greater than 0.");
        this.Rows = rows;
        this.Cols = cols;

        this.data = new int[rows, cols];
        Fill(0);
        // 这里必须得Rebuild，data创建后默认全部为0，Fill中是用Set来设置值的，如果原来就是0的话，设置成0并不会将其加入emptyCells
        RebuildEmptyCells();
    }

    #region 访问
    // 复制一份
    public int[,] Data()
    {
        int[,] copy = new int[Rows, Cols];
        Array.Copy(data, copy, data.Length);
        return copy;
    }

    public int Get(int row, int col)
    {
        CheckBounds(row, col);
        return data[row, col];
    }

    // 复制一份
    public int[] GetRow(int row)
    {
        CheckBounds(row, 0);
        int[] res = new int[Cols];
        for (int col = 0; col < Cols; ++col)
        {
            res[col] = this.data[row, col];
        }
        return res;
    }

    public bool IsFull()
    {
        return emptyCells.Count == 0;
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

    public void Display()
    {
        for (int r = 0; r < Rows; ++r)
        {
            Debug.Log(string.Join(' ', GetRow(r)));
        }
        Debug.Log("------------------------------------");
    }
    #endregion
    
    #region 修改

    // 原来非零修改后为0会将其加入emptyCells，相反会将其移除
    public void Set(int row, int col, int val)
    {
        CheckBounds(row, col);
        if (data[row, col] == 0 && val != 0) {
            emptyCells.Remove(new Vector2Int(row, col));
        } else if (data[row, col] != 0 && val == 0) {
            emptyCells.Add(new Vector2Int(row, col));
        }
        data[row, col] = val;
    }

    // 内部通过Set赋值
    public void Fill(int val)
    {
        for (int r = 0; r < Rows; ++r)
        {
            for (int c = 0; c < Cols; ++c)
            {
                Set(r, c, val);
            }
        }
    }
    #endregion

    #region 旋转
    public void Rotate90Clockwise()
    {
        int[,] newMap = new int[Cols, Rows];
        for (int r = 0; r < Rows; ++r)
        {
            for (int c = 0; c < Cols; ++c)
            {
                newMap[c, Rows - 1 - r] = data[r, c];
            }
        }
        data = newMap;

        (Cols, Rows) = (Rows, Cols);

        this.RebuildEmptyCells();
    }

    public void Rotate180Clockwise()
    {
        int[,] newMap = new int[Rows, Cols];
        for (int r = 0; r < Rows; ++r)
        {
            for (int c = 0; c < Cols; ++c)
            {
                newMap[Rows - 1 - r, Cols - 1 - c] = data[r, c];
            }
        }
        data = newMap;
        this.RebuildEmptyCells();
    }

    public void Rotate270Clockwise()
    {
        int[,] newMap = new int[Cols, Rows];
        for (int r = 0; r < Rows; ++r)
        {
            for (int c = 0; c < Cols; ++c)
            {
                newMap[Cols - 1 - c, r] = data[r, c];
            }
        }
        data = newMap;

        (Cols, Rows) = (Rows, Cols);

        this.RebuildEmptyCells();
    }

    // 旋转的是数组下的坐标，不是常规的笛卡尔坐标系
    // 数组下的(0,0)类似笛卡尔坐标系下的(1, 1)，旋转是会变化的
    public Vector2Int Rotate90Clockwise(Vector2Int coord)
    {
        return new Vector2Int(coord.y, Rows - 1 - coord.x);
    }

    public Vector2Int Rotate180Clockwise(Vector2Int coord)
    {
        return new Vector2Int(Rows - 1 - coord.x, Cols - 1 - coord.y);
    }

    public Vector2Int Rotate270Clockwise(Vector2Int coord)
    {
        return new Vector2Int(Cols - 1 - coord.y, coord.x);
    }
    #endregion

    private void RebuildEmptyCells()
    {
        this.emptyCells.Clear();
        for (int r = 0; r < Rows; ++r)
        {
            for (int c = 0; c < Cols; ++c)
            {
                if (data[r, c] == 0)
                {
                    emptyCells.Add(new Vector2Int(r, c));
                }
            }
        }
    }

    private void CheckBounds(int row, int col)
    {
        if (row < 0 || row >= Rows || col < 0 || col >= Cols)
        {
            throw new IndexOutOfRangeException("Row or column is out of bounds.");
        }
    }
}