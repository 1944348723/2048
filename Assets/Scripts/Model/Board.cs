using System;
using System.Collections.Generic;
using UnityEngine;

enum Rotation { Clockwise90, Clockwise180, Clockwise270, None };

public class Board
{
    private int rows;
    private int cols;
    // TODO: 可配置
    private readonly int[] generatableNumbers = new int[] { 2, 4 };

    // 注入依赖数据结构
    private GridMap gridMap;

    private bool initialized = false;
    private Rotation currentRotation = Rotation.None;
    private readonly List<TileAction> tileActions = new();

    public event System.Action<List<TileAction>> Ticked;  // number, row, col
    public event System.Action<int> Merged;
    
    public bool Init(GridMap gridMap)
    {
        if (initialized) {
            Debug.LogError("Board has already been initialized.");
            return false;
        }

        this.gridMap = gridMap;
        this.rows = gridMap.GetRowCount();
        this.cols = gridMap.GetColCount();
        // 自己订阅自己的事件，不用解除订阅
        this.Ticked += ClearTileActions;
        this.initialized = true;
        return true;
    }

    public void StartGame()
    {
        EnsureInitialized();

        this.gridMap.Fill(0);
        for (int i = 0; i < 2; ++i)
        {
            GenerateRandomNumber();
        }
        // 日志打印棋盘
        gridMap.Display();
        
        // 通知表现层绘制
        Ticked?.Invoke(new List<TileAction>(tileActions));
    }

    public bool IsGameOver()
    {
        return this.gridMap.IsFull() && !HasConnectedSameTile();
    }

    public bool TryMove(Direction dir)
    {
        bool hasChanged = false;
        switch (dir)
        {
            case Direction.Up:
                this.gridMap.Rotate270Clockwise();
                this.currentRotation = Rotation.Clockwise270;
                hasChanged = this.PushLeft();
                this.gridMap.Rotate90Clockwise();
                this.currentRotation = Rotation.None;
                break;
            case Direction.Down:
                this.gridMap.Rotate90Clockwise();
                this.currentRotation = Rotation.Clockwise90;
                hasChanged = this.PushLeft();
                this.gridMap.Rotate270Clockwise();
                this.currentRotation = Rotation.None;
                break;
            case Direction.Left:
                hasChanged = PushLeft();
                break;
            case Direction.Right:
                this.gridMap.Rotate180Clockwise();
                this.currentRotation = Rotation.Clockwise180;
                hasChanged = this.PushLeft();
                this.gridMap.Rotate180Clockwise();
                this.currentRotation = Rotation.None;
                break;
        }
        if (hasChanged)
        {
            GenerateRandomNumber();
            // 绘制
            gridMap.Display();
            Ticked?.Invoke(new List<TileAction>(tileActions));
        }
        return hasChanged;
    }

    private bool HasConnectedSameTile()
    {
        int[,] data = this.gridMap.Data();

        // 横向
        for (int r = 0; r < rows; ++r)
        {
            for (int c = 0; c < cols - 1; ++c)
            {
                if (data[r,c] == data[r, c + 1])
                {
                    return true;
                }
            }
        }
        // 纵向
        for (int c = 0; c < cols; ++c)
        {
            for (int r = 0; r < rows - 1; ++r)
            {
                if (data[r,c] == data[r + 1, c])
                {
                    return true;
                }
            }
        }
        return false;
    }

    // 返回值为是否有变化
    private bool PushLeft()
    {
        bool hasChanged = false;

        for (int r = 0; r < rows; ++r)
        {
            int[] row = this.gridMap.GetRow(r);
            hasChanged |= PushLine(row, r);

            for (int c = 0; c < row.Length; ++c)
            {
                this.gridMap.Set(r, c, row[c]);
            }
        }
        return hasChanged;
    }

    // public方便测试，按理说应该是private
    // 往左，返回值为是否有变化
    public bool PushLine(int[] line, int row = 0)
    {
        bool hasChanged = false;
        // 非零数字提取到前面，并记录原始列索引，方便后续生成TileAction
        // <val, source column>
        var entries = new List<(int val, int sourceCol)>();
        for (int col = 0; col < line.Length; ++col)
        {
            if (line[col] != 0)
            {
                entries.Add((line[col], col));
            }
        }

        
        // 所有非零数字都在entries中，对于任意一个非零数字，要么向左合并，要么向左移动，要么不动
        // 每次尝试取read和read + 1处的数字，首先判断是否是合并。如果不是，根据sourceCol和write的关系判断是移动还是不动
        // 合并时read+2跳过两个合并的数字，移动时read+1跳过一个被移动的数字，不动时read+1跳过一个不动的数字
        int write = 0;
        for (int read = 0; read < entries.Count; ++write)
        {
            int val = entries[read].val;
            int sourceCol = entries[read].sourceCol;

            // 合并
            if (read + 1 < entries.Count && entries[read + 1].val == val)
            {
                hasChanged = true;
                line[write] = val * 2;
                read += 2;

                Vector2Int from1 = ToCoordinateBeforeRotation(new Vector2Int(row, sourceCol));
                Vector2Int from2 = ToCoordinateBeforeRotation(new Vector2Int(row, entries[read - 1].sourceCol));
                Vector2Int to = ToCoordinateBeforeRotation(new Vector2Int(row, write));

                this.tileActions.Add(TileAction.Merge(from1, from2, to, val * 2));
                this.Merged?.Invoke(2 * val);
            }
            else if (sourceCol != write) // 移动
            {
                hasChanged = true;
                line[write] = val;
                ++read;

                Vector2Int from = ToCoordinateBeforeRotation(new Vector2Int(row, sourceCol));
                Vector2Int to = ToCoordinateBeforeRotation(new Vector2Int(row, write));
                this.tileActions.Add(TileAction.Move(from, to));
            } else // 不动
            {
                ++read;
            }
        }
        for (; write < line.Length; ++write)
        {
            line[write] = 0;
        }
        return hasChanged;
    }

    private void GenerateRandomNumber()
    {
        if (!this.gridMap.TryGetRandomEmptyCoordinate(out Vector2Int coordinate))
        {
            Debug.LogError("No empty coordinate found.");
            return;
        }
        int randIndex = UnityEngine.Random.Range(0, generatableNumbers.Length);
        int num = generatableNumbers[randIndex];
        this.gridMap.Set(coordinate.x, coordinate.y, num);

        this.tileActions.Add(TileAction.Spawn(coordinate, num));
    }

    private void ClearTileActions(List<TileAction> actions)
    {
        // 不要清除传入的actions，传入的是给view层的
        this.tileActions.Clear();
    }

    private Vector2Int ToCoordinateBeforeRotation(Vector2Int coord)
    {
        switch (this.currentRotation)
        {
            case Rotation.Clockwise90:
                return this.gridMap.Rotate270Clockwise(coord);
            case Rotation.Clockwise180:
                return this.gridMap.Rotate180Clockwise(coord);
            case Rotation.Clockwise270:
                return this.gridMap.Rotate90Clockwise(coord);
            case Rotation.None:
                return coord;
        }
        return coord;
    }

    private void EnsureInitialized()
    {
        if (!initialized) throw new InvalidOperationException("Board has not been initialized.");
    }
}
