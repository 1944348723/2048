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


    public bool TryMove(Direction dir)
    {
        return ExecuteMove(dir, true);
    }

    public bool CanMove(Direction dir)
    {
        return ExecuteMove(dir, false);
    }

    private bool ExecuteMove(Direction dir, bool takeEffect)
    {
        bool hasChanged = false;
        switch (dir)
        {
            case Direction.Up:
                this.gridMap.Rotate270Clockwise();
                this.currentRotation = Rotation.Clockwise270;
                hasChanged = this.PushLeft(takeEffect);
                this.gridMap.Rotate90Clockwise();
                this.currentRotation = Rotation.None;
                break;
            case Direction.Down:
                this.gridMap.Rotate90Clockwise();
                this.currentRotation = Rotation.Clockwise90;
                hasChanged = this.PushLeft(takeEffect);
                this.gridMap.Rotate270Clockwise();
                this.currentRotation = Rotation.None;
                break;
            case Direction.Left:
                hasChanged = PushLeft(takeEffect);
                break;
            case Direction.Right:
                this.gridMap.Rotate180Clockwise();
                this.currentRotation = Rotation.Clockwise180;
                hasChanged = this.PushLeft(takeEffect);
                this.gridMap.Rotate180Clockwise();
                this.currentRotation = Rotation.None;
                break;
        }
        if (hasChanged && takeEffect)
        {
            GenerateRandomNumber();
            // 绘制
            gridMap.Display();
            Ticked?.Invoke(new List<TileAction>(tileActions));
        }
        return hasChanged;
    }

    // 返回值为是否有变化
    private bool PushLeft(bool takeEffect)
    {
        bool hasChanged = false;

        for (int r = 0; r < rows; ++r)
        {
            int[] row = this.gridMap.GetRow(r);
            int[] originalRow = (int[])row.Clone();
            PushLine(row, r, takeEffect);
            if (!hasChanged && !System.Linq.Enumerable.SequenceEqual(row, originalRow))
            {
                hasChanged = true;
            }

            if (!takeEffect) continue;  // 不生效的话跳过写回
            for (int c = 0; c < row.Length; ++c)
            {
                this.gridMap.Set(r, c, row[c]);
            }
        }
        return hasChanged;
    }

    // public方便测试，按理说应该是private
    // 往左，返回值为是否有变化
    public void PushLine(int[] arr, int row = 0, bool takeEffect = true)
    {
        // 非零数字提取到前面，并记录原始列索引，方便后续生成TileAction
        // <val, source column>
        var entries = new List<(int val, int sourceCol)>();
        for (int col = 0; col < arr.Length; ++col)
        {
            if (arr[col] != 0)
            {
                entries.Add((arr[col], col));
            }
        }

        // 处理合并和移动
        int write = 0;
        for (int read = 0; read < entries.Count; ++write)
        {
            // 保证read在数组范围内且read处元素有效
            int val = entries[read].val;
            int sourceCol = entries[read].sourceCol;

            // 合并
            if (read + 1 < entries.Count && entries[read + 1].val == val)
            {
                arr[write] = val * 2;
                read += 2;

                if (!takeEffect) continue;
                Vector2Int from1 = ToCoordinateBeforeRotation(new Vector2Int(row, sourceCol));
                Vector2Int from2 = ToCoordinateBeforeRotation(new Vector2Int(row, entries[read - 1].sourceCol));
                Vector2Int to = ToCoordinateBeforeRotation(new Vector2Int(row, write));

                this.tileActions.Add(TileAction.Merge(from1, from2, to, val * 2));
                this.Merged?.Invoke(2 * val);
            }
            else  // 移动
            {
                arr[write] = val;
                ++read;

                if (!takeEffect) continue;
                Vector2Int from = ToCoordinateBeforeRotation(new Vector2Int(row, sourceCol));
                Vector2Int to = ToCoordinateBeforeRotation(new Vector2Int(row, write));
                this.tileActions.Add(TileAction.Move(from, to));
            }
        }
        for (; write < arr.Length; ++write)
        {
            arr[write] = 0;
        }
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
        // 不要清除传入的actions，传入的是复制给view层的
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
