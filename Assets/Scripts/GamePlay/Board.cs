using System;
using System.Collections.Generic;
using UnityEngine;

enum Rotation { Clockwise90, Clockwise180, Clockwise270, None };

public class Board
{
    public int Score { get; private set; } = 0;

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
            int[] currentRow = this.gridMap.GetRow(r);
            LineResolveResult result = LineResolver.Resolve(currentRow);

            // 应用操作结果
            hasChanged |= result.HasChanged;
            int[] newLine = result.NewLine;
            AddToTileActions(result.Operations, r);
            AddScore(result.Operations);

            for (int c = 0; c < currentRow.Length; ++c)
            {
                this.gridMap.Set(r, c, newLine[c]);
            }
        }
        return hasChanged;
    }

    private void AddToTileActions(IReadOnlyList<LineOperation> operations, int row)
    {
        foreach (var op in operations) {
            if (op.Type == LineOperationType.Move)
            {
                Vector2Int from = ToCoordinateBeforeRotation(new(row, op.From1));
                Vector2Int to = ToCoordinateBeforeRotation(new(row, op.To));
                tileActions.Add(TileAction.Move(from, to));
            } else if (op.Type == LineOperationType.Merge)
            {
                Vector2Int from1 = ToCoordinateBeforeRotation(new(row, op.From1));
                Vector2Int from2 = ToCoordinateBeforeRotation(new(row, op.From2));
                Vector2Int to = ToCoordinateBeforeRotation(new(row, op.To));
                tileActions.Add(TileAction.Merge(from1, from2, to, op.Value));
            }
        }
    }

    private void AddScore(IReadOnlyList<LineOperation> operations)
    {
        foreach (var op in operations)
        {
            if (op.Type == LineOperationType.Merge)
            {
                Score += op.Value;
            }
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
