using System.Collections.Generic;
using UnityEngine;

enum Rotation { Clockwise90, Clockwise180, Clockwise270, None };
public class Board
{
    private const int rows = 4;
    private const int cols = 4;
    private GridMap gridMap;
    private int[] generatableNumbers = new int[] { 2, 4 };
    private Rotation currentRotation = Rotation.None;

    public event System.Action<int, int, int> OnSpawn;  // number, row, col
    
    public void Init(GridMap gridMap)
    {
        this.gridMap = gridMap;
    }

    public void StartGame()
    {
        this.gridMap.Fill(0);
        for (int i = 0; i < 2; ++i)
        {
            GenerateRandomNumber();
        }
        gridMap.Display();
    }

    private void GenerateRandomNumber()
    {
        // 逻辑上生成数字
        int randIndex = Random.Range(0, generatableNumbers.Length);
        int num = generatableNumbers[randIndex];
        Vector2Int coordinate = this.gridMap.GetRandomEmptyCoordinate();
        this.gridMap.Set(coordinate.x, coordinate.y, num);

        // 通知视图层
        OnSpawn?.Invoke(num, coordinate.x, coordinate.y);
    }

    public void Push(Direction dir)
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
        }
        gridMap.Display();
    }


    // 返回值为是否有变化
    private bool PushLeft()
    {
        bool hasChanged = false;

        for (int r = 0; r < rows; ++r)
        {
            int[] row = this.gridMap.GetRow(r);
            int[] originalRow = (int[])row.Clone();
            PushLine(row);
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
    public static void PushLine(int[] arr, int row = 0)
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

                // TODO
            } else              // 移动
            {
                arr[write] = val;
                // TODO
            }
            ++write;
        }
        for (; write < arr.Length; ++write)
        {
            arr[write] = 0;
        }
    }
}
