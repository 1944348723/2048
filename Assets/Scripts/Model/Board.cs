using System;
using System.Collections.Generic;
using UnityEngine;

enum Rotation { Clockwise90, Clockwise180, Clockwise270, None };

public class Board
{
    public int Score { get; private set; }

    // 注入依赖数据结构
    private readonly GridMap gridMap;
    private readonly int rows;
    private readonly int cols;
    // TODO: 可配置
    private readonly int[] generatableNumbers = new int[] { 2, 4 };

    private Rotation currentRotation = Rotation.None;
    private readonly List<TileAction> tileActions = new();

    public Board(GridMap gridMap)
    {
        this.gridMap = gridMap ?? throw new ArgumentNullException(nameof(gridMap));
        this.rows = gridMap.Rows;
        this.cols = gridMap.Cols;
    }

    public List<TileAction> StartGame()
    {
        this.Score = 0;
        this.gridMap.Fill(0);
        for (int i = 0; i < 2; ++i)
        {
            GenerateRandomNumber();
        }
        // 日志打印棋盘
        gridMap.Display();

        return GetActions();
    }

    public bool IsGameOver()
    {
        return this.gridMap.IsFull() && !HasConnectedSameTile();
    }

    public List<TileAction> TryMove(Direction dir)
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
        }

        return GetActions();
    }

    private List<TileAction> GetActions()
    {
        List<TileAction> actionsToReturn = new(this.tileActions);
        this.tileActions.Clear();
        return actionsToReturn;
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

    // 返回这次操作是否有变化
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
                Score += val * 2;
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

    private Vector2Int ToCoordinateBeforeRotation(Vector2Int coord)
    {
        return this.currentRotation switch
        {
            Rotation.Clockwise90 => this.gridMap.Rotate270Clockwise(coord),
            Rotation.Clockwise180 => this.gridMap.Rotate180Clockwise(coord),
            Rotation.Clockwise270 => this.gridMap.Rotate90Clockwise(coord),
            Rotation.None => coord,
            _ => throw new InvalidOperationException("Invalid Rotation State"),
        };
    }
}
