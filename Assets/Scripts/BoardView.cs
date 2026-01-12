using UnityEngine;

public class BoardView : MonoBehaviour
{
    [SerializeField] private Vector2 gap = new Vector2(0f, 0f);
    [SerializeField] private GameObject CellBgPrefab;
    [SerializeField] private GameObject NumberTilePrefab;

    private GridMap gridMap;
    private Board board;
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

    public void Init(GridMap gridMap)
    {
        this.gridMap = gridMap;
        float tileWidth = CellBgPrefab.GetComponent<RectTransform>().rect.width;
        float tileHeight = CellBgPrefab.GetComponent<RectTransform>().rect.height;
        this.gridMap.Init(new Vector2(tileWidth, tileHeight), new Vector2(gap.x, gap.y), this.transform.position);
    }

    public void Bind(Board board)
    {
        this.board = board;
        this.board.OnSpawn += SpawnNumberTile;
    }

    private void SpawnNumberTile(int num, int row, int col)
    {
        GameObject tile = Instantiate(NumberTilePrefab, numberTileContainer.transform);
        NumberTileView tileComponent = tile.GetComponent<NumberTileView>();
        tileComponent.SetNumber(num);
        tileComponent.transform.position = gridMap.GridToWorld(row, col);
    }
}
