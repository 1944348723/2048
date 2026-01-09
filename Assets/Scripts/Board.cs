using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject CellBgPrefab;
    [SerializeField] private GameObject NumberTilePrefab;
    [SerializeField] private Vector2 gap = new Vector2(0f, 0f);

    private const int rows = 4;
    private const int cols = 4;
    private GridMap gridMap;

    void Start()
    {
        float tileWidth = CellBgPrefab.GetComponent<SpriteRenderer>().bounds.size.x;
        float tileHeight = CellBgPrefab.GetComponent<SpriteRenderer>().bounds.size.y;

        this.gridMap = new GridMap(rows, cols);
        this.gridMap.Init(new Vector2(tileWidth, tileHeight), new Vector2(gap.x, gap.y), Vector2.zero);
        InitBoard();
    }

    private void InitBoard()
    {
        for (int r = 0; r < rows; ++r)
        {
            for (int c = 0; c < cols; ++c)
            {
                gridMap.Set(r, c, -1);
                GameObject cellBg = Instantiate(CellBgPrefab, this.transform);
                cellBg.transform.position = gridMap.GridToWorld(r, c);
            }
        }
    }
}
