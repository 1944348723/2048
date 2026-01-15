using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

public enum TileActionType { Move, Merge, Spawn };
/*
    Move - from, to
    Merge - from1, from2, to, val
    Spawn - to, val
*/
public struct TileAction
{

    public Vector2Int from1;
    public Vector2Int from2;
    public Vector2Int to;
    public int val;
    public TileActionType actionType;
}

enum Rotation { Clockwise90, Clockwise180, Clockwise270, None };
public class Board
{
    // 固有
    private const int rows = 4;
    private const int cols = 4;
    private int[] generatableNumbers = new int[] { 2, 4 };

    // 注入
    private GridMap gridMap;

    // 运行时
    public event System.Action<List<TileAction>> OnTick;  // number, row, col
    public event System.Action<int> OnMerge;
    private Rotation currentRotation = Rotation.None;
    // 每次操作后清空
    private List<TileAction> tileActions = new();
    
    public void Init(GridMap gridMap)
    {
        this.gridMap = gridMap;
        this.OnTick += ClearTileActions;
    }

    public void StartGame()
    {
        this.gridMap.Fill(0);
        for (int i = 0; i < 2; ++i)
        {
            GenerateRandomNumber();
        }
        gridMap.Display();
        
        // 绘制
        OnTick?.Invoke(new List<TileAction>(tileActions));
    }

    private void GenerateRandomNumber()
    {
        // 逻辑上生成数字
        int randIndex = Random.Range(0, generatableNumbers.Length);
        int num = generatableNumbers[randIndex];
        Vector2Int coordinate = this.gridMap.GetRandomEmptyCoordinate();
        this.gridMap.Set(coordinate.x, coordinate.y, num);

        this.tileActions.Add(new TileAction
        {
            to = coordinate,
            val = num,
            actionType = TileActionType.Spawn
        });
    }

    public bool Push(Direction dir)
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
            OnTick?.Invoke(new List<TileAction>(tileActions));
        }
        return hasChanged;
    }


    // 返回值为是否有变化
    private bool PushLeft()
    {
        bool hasChanged = false;

        for (int r = 0; r < rows; ++r)
        {
            int[] row = this.gridMap.GetRow(r);
            int[] originalRow = (int[])row.Clone();
            PushLine(row, r);
            for (int c = 0; c < row.Length; ++c)
            {
                gridMap.Set(r, c, row[c]);
            }
            if (!hasChanged && !System.Linq.Enumerable.SequenceEqual(row, originalRow))
            {
                hasChanged = true;
            }
        }
        return hasChanged;
    }

    // public方便测试，按理说应该是private
    // 往左，返回值为是否有变化
    public void PushLine(int[] arr, int row = 0)
    {
        // 非零数字提取到前面
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
        for (int read = 0; read < entries.Count; ++read)
        {
            // 保证read在数组范围内且read处元素有效
            int val = entries[read].val;
            int sourceCol = entries[read].sourceCol;

            // 合并
            if (read + 1 < entries.Count && entries[read + 1].val == val)
            {
                arr[write] = val * 2;
                ++read;

                var mergeAction = new TileAction
                {
                    from1 = ToCoordinateBeforeRotation(new Vector2Int(row, sourceCol)),
                    from2 = ToCoordinateBeforeRotation(new Vector2Int(row, entries[read].sourceCol)),
                    to = ToCoordinateBeforeRotation(new Vector2Int(row, write)),
                    val = val * 2,
                    actionType = TileActionType.Merge
                };
                this.tileActions.Add(mergeAction);
                this.OnMerge?.Invoke(2 * val);
            }
            else  // 移动
            {
                arr[write] = val;
                var moveAction = new TileAction
                {
                    from1 = ToCoordinateBeforeRotation(new Vector2Int(row, sourceCol)),
                    to = ToCoordinateBeforeRotation(new Vector2Int(row, write)),
                    actionType = TileActionType.Move
                };
                this.tileActions.Add(moveAction);
            }
            ++write;
        }
        for (; write < arr.Length; ++write)
        {
            arr[write] = 0;
        }
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
}
