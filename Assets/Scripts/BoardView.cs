using System.Collections.Generic;
using UnityEngine;

public class BoardView : MonoBehaviour
{
    [SerializeField] private Vector2 gap = new Vector2(0f, 0f);
    [SerializeField] private GameObject CellBgPrefab;
    [SerializeField] private GameObject NumberTilePrefab;

    private GridMap gridMap;
    private GameObject tileBgContainer;
    private GameObject numberTileContainer;
    private LinkedList<NumberTileView> freeTileViews;
    private NumberTileView[,] activeTileViews;

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
        ConfigureGridLayout();
        GenerateTileBgs();
        PreCreateTileViews();
    }

    public void Bind(Board board)
    {
        board.OnTick += UpdateView;
    }

    private void ConfigureGridLayout()
    {
        float tileWidth = CellBgPrefab.GetComponent<RectTransform>().rect.width;
        float tileHeight = CellBgPrefab.GetComponent<RectTransform>().rect.height;
        this.gridMap.ConfigureLayout(new Vector2(tileWidth, tileHeight), new Vector2(gap.x, gap.y), this.transform.position);
    }

    private void GenerateTileBgs()
    {
        for (int r = 0; r < gridMap.GetRowCount(); ++r)
        {
            for (int c = 0; c < gridMap.GetColCount(); ++c)
            {
                GameObject tileBg = Instantiate(CellBgPrefab, tileBgContainer.transform);
                tileBg.transform.position = gridMap.GridToWorld(r, c);
            }
        }
    }

    private void PreCreateTileViews()
    {
        freeTileViews = new LinkedList<NumberTileView>();
        activeTileViews = new NumberTileView[gridMap.GetRowCount(), gridMap.GetColCount()];
        for (int r = 0; r < gridMap.GetRowCount(); ++r)
        {
            for (int c = 0; c < gridMap.GetColCount(); ++c)
            {
                GameObject tile = Instantiate(NumberTilePrefab, numberTileContainer.transform);
                NumberTileView tileComponent = tile.GetComponent<NumberTileView>();
                freeTileViews.AddFirst(tileComponent);
                tile.SetActive(false);
            }
        }
    }

    private void UpdateView(List<TileAction> actions)
    {
        foreach (var action in actions)
        {
            switch (action.actionType)
            {
                case TileActionType.Spawn:
                    {
                        var tileView = freeTileViews.First.Value;
                        freeTileViews.RemoveFirst();
                        tileView.SetNumber(action.val);
                        tileView.transform.position = gridMap.GridToWorld(action.to.x, action.to.y);
                        activeTileViews[action.to.x, action.to.y] = tileView;
                        tileView.gameObject.SetActive(true);
                    }
                    break;
                case TileActionType.Move:
                    {
                        var tileView = activeTileViews[action.from1.x, action.from1.y];
                        activeTileViews[action.from1.x, action.from1.y] = null;
                        activeTileViews[action.to.x, action.to.y] = tileView;
                        tileView.transform.position = gridMap.GridToWorld(action.to.x, action.to.y);
                    }
                    break;
                case TileActionType.Merge:
                    {
                        var tileView1 = activeTileViews[action.from1.x, action.from1.y];
                        var tileView2 = activeTileViews[action.from2.x, action.from2.y];
                        activeTileViews[action.from1.x, action.from1.y] = null;
                        activeTileViews[action.from2.x, action.from2.y] = null;
                        activeTileViews[action.to.x, action.to.y] = tileView1;
                        tileView1.transform.position = gridMap.GridToWorld(action.to.x, action.to.y);
                        tileView1.SetNumber(action.val);
                        tileView2.gameObject.SetActive(false);
                        freeTileViews.AddLast(tileView2);
                    }
                    break;
            }
        }
    }
}
