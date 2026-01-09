using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject CellBgPrefab;
    [SerializeField] private GameObject NumberTilePrefab;
    [SerializeField] private Vector2 gap = new Vector2(0f, 0f);
    [SerializeField] private Color[] numberTileColors;

    private const int rows = 4;
    private const int cols = 4;
    private GridMap gridMap;
    private int[] generatableNumbers = new int[] { 2, 4 };
    private GameObject tileBgContainer;
    private GameObject numberTileContainer;

    void Awake()
    {
        tileBgContainer = new GameObject("TileBgContainer");
        tileBgContainer.transform.parent = this.transform;
        tileBgContainer.transform.localPosition = Vector3.zero;
        numberTileContainer = new GameObject("NumberTileContainer");
        numberTileContainer.transform.parent = this.transform;
        numberTileContainer.transform.localPosition = Vector3.zero;
    }
    void Start()
    {
        float tileWidth = CellBgPrefab.GetComponent<RectTransform>().rect.width;
        float tileHeight = CellBgPrefab.GetComponent<RectTransform>().rect.height;

        this.gridMap = new GridMap(rows, cols);
        this.gridMap.Init(new Vector2(tileWidth, tileHeight), new Vector2(gap.x, gap.y), this.transform.position);
        InitBoard();
        
        for (int i = 0; i < 16; ++i)
        {
            GenerateRandomNumberTile();
        }
    }

    private void InitBoard()
    {
        for (int r = 0; r < rows; ++r)
        {
            for (int c = 0; c < cols; ++c)
            {
                gridMap.Set(r, c, -1);
                GameObject cellBg = Instantiate(CellBgPrefab, tileBgContainer.transform);
                cellBg.transform.position = gridMap.GridToWorld(r, c);
            }
        }
    }

    private Color GetNumColor(int num)
    {
        switch (num)
        {
            case 2: return numberTileColors[0];
            case 4: return numberTileColors[1];
            case 8: return numberTileColors[2];
            case 16: return numberTileColors[3];
            case 32: return numberTileColors[4];
            case 64: return numberTileColors[5];
            case 128: return numberTileColors[6];
            case 256: return numberTileColors[7];
            case 512: return numberTileColors[8];
            case 1024: return numberTileColors[9];
            case 2048: return numberTileColors[10];
            default: return numberTileColors[11];
        }
    }

    private GameObject CreateNumberTile(int num)
    {
        GameObject tile = Instantiate(NumberTilePrefab, numberTileContainer.transform);
        NumberTile numberTile = tile.GetComponent<NumberTile>();
        numberTile.SetNumber(num);
        numberTile.SetNumberColor(GetNumColor(num));
        return tile;
    }

    private void GenerateRandomNumberTile()
    {
        int randIndex = Random.Range(0, generatableNumbers.Length);
        int num = generatableNumbers[randIndex];
        Vector2Int position = this.gridMap.GetRandomEmptyPosition();

        gridMap.Set(position.x, position.y, num);

        GameObject tile = CreateNumberTile(num);
        tile.transform.position = gridMap.GridToWorld(position.x, position.y);
    }
}
