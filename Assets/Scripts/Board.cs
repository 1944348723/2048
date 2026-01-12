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
    public static void PushLine(int[] arr)
    {
        int l = 0, r = 1;
        int cur = 0;        // 下一个填入位置

        int len = arr.Length;
        do
        {
            // 每次尝试寻找两个有效数字
            while (l < len && arr[l] == 0) ++l;
            r = l + 1;
            while (r < len && arr[r] == 0) ++r;

            // 找到两个
            if (l < len && r < len)
            {
                if (arr[l] == arr[r])
                {
                    int valL = arr[l];
                    arr[l] = 0;arr[r] = 0;
                    arr[cur++] = 2 * valL;
                    l = r + 1;
                    r = l + 1;
                    // TODO: 通知视图层
                } else
                {
                    // 为了方便就不判断当前位置和目标位置是否相同了，直接统一化处理
                    int valL = arr[l], valR = arr[r];
                    arr[l] = 0;arr[r] = 0;
                    arr[cur++] = valL;
                    arr[cur] = valR;
                    l = cur;
                    r = l + 1;
                    // TODO
                }
            } else if (l < len && r >= len)     // 找到一个
            {
                int val = arr[l];
                arr[l] = 0;
                arr[cur++] = val;
                l = r;
                // TODO
            } 
            // 一个都没找到不需要处理，自然会退出循环
        } while (l < len);
    }
}
