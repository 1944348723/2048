using UnityEngine;

public class Board
{
    private const int rows = 4;
    private const int cols = 4;
    private GridMap gridMap;
    private int[] generatableNumbers = new int[] { 2, 4 };

    public event System.Action<int, int, int> OnSpawn;  // number, row, col
    
    public void Init(GridMap gridMap)
    {
        this.gridMap = gridMap;
    }

    public void StartGame()
    {
        this.gridMap.Fill(0);
        for (int i = 0; i < 16; ++i)
        {
            GenerateRandomNumber();
        }
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
}
