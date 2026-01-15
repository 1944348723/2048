using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardView : MonoBehaviour
{
    [SerializeField] private Vector2 gap = new Vector2(0f, 0f);
    [SerializeField] private GameObject CellBgPrefab;
    [SerializeField] private GameObject NumberTilePrefab;
    [SerializeField] private GameObject tileBgContainer;
    [SerializeField] private GameObject numberTileContainer;
    [SerializeField] private TileColor[] colors;

    public event System.Action OnAnimationFinished;

    private GridMap gridMap;
    private LinkedList<NumberTileView> freeTileViews;
    private NumberTileView[,] activeTileViews;
    private float moveDuration = 0.1f;

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

    public void Reset()
    {
        foreach (var tile in activeTileViews)
        {
            if (tile)
            {
                tile.gameObject.SetActive(false);
                freeTileViews.AddFirst(tile);
            }
        }
    }

    public void SetAnimationDuration(float duration)
    {
        this.moveDuration = duration;
    }

    private void ConfigureGridLayout()
    {
        float tileWidth = CellBgPrefab.GetComponent<RectTransform>().rect.width;
        float tileHeight = CellBgPrefab.GetComponent<RectTransform>().rect.height;
        this.gridMap.ConfigureLayout(new Vector2(tileWidth, tileHeight), new Vector2(gap.x, gap.y), Vector2.zero);
    }

    private void GenerateTileBgs()
    {
        for (int r = 0; r < gridMap.GetRowCount(); ++r)
        {
            for (int c = 0; c < gridMap.GetColCount(); ++c)
            {
                GameObject tileBg = Instantiate(CellBgPrefab);
                tileBg.transform.SetParent(tileBgContainer.transform, false);
                tileBg.transform.localPosition = gridMap.GridToPosition(r, c);
            }
        }
    }

    private void PreCreateTileViews()
    {
        freeTileViews = new LinkedList<NumberTileView>();
        activeTileViews = new NumberTileView[gridMap.GetRowCount(), gridMap.GetColCount()];
        int count = gridMap.GetRowCount() * gridMap.GetColCount();
        // 多创建几个，假设棋盘摆满2，这时候移动的话，合并需要额外8个，生成需要额外1个，所以最极端情况下需要额外9个
        for (int i = 0; i < count + 9; ++i)
        {
            GameObject tile = Instantiate(NumberTilePrefab);
            tile.transform.SetParent(numberTileContainer.transform, false);
            NumberTileView tileComponent = tile.GetComponent<NumberTileView>();
            freeTileViews.AddFirst(tileComponent);
            tile.SetActive(false);
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
                        var tileView = GetTileView(action.val);
                        tileView.transform.localPosition = gridMap.GridToPosition(action.to.x, action.to.y);
                        activeTileViews[action.to.x, action.to.y] = tileView;
                        tileView.gameObject.SetActive(true);
                    }
                    break;
                case TileActionType.Move:
                    {
                        var tileView = activeTileViews[action.from1.x, action.from1.y];
                        activeTileViews[action.from1.x, action.from1.y] = null;
                        activeTileViews[action.to.x, action.to.y] = tileView;

                        Move(tileView.transform, tileView.transform.localPosition, gridMap.GridToPosition(action.to.x, action.to.y), moveDuration);
                        // tileView.transform.position = gridMap.GridToWorld(action.to.x, action.to.y);
                    }
                    break;
                case TileActionType.Merge:
                    {
                        var tileView1 = activeTileViews[action.from1.x, action.from1.y];
                        var tileView2 = activeTileViews[action.from2.x, action.from2.y];
                        activeTileViews[action.from1.x, action.from1.y] = null;
                        activeTileViews[action.from2.x, action.from2.y] = null;
                        Move(tileView1.transform, tileView1.transform.localPosition, gridMap.GridToPosition(action.to.x, action.to.y), moveDuration);
                        Move(tileView2.transform, tileView2.transform.localPosition, gridMap.GridToPosition(action.to.x, action.to.y), moveDuration);
                        StartCoroutine(DelayAction(() => { tileView1.gameObject.SetActive(false); freeTileViews.AddFirst(tileView1); }, moveDuration));
                        StartCoroutine(DelayAction(() => { tileView2.gameObject.SetActive(false); freeTileViews.AddFirst(tileView2); }, moveDuration));

                        var newtileView = GetTileView(action.val);
                        activeTileViews[action.to.x, action.to.y] = newtileView;
                        newtileView.transform.localPosition = gridMap.GridToPosition(action.to.x, action.to.y);
                        StartCoroutine(DelayAction(() => { newtileView.gameObject.SetActive(true); }, moveDuration));
                    }
                    break;
            }
        }
        StartCoroutine(DelayAction(() => { this.OnAnimationFinished?.Invoke(); }, moveDuration));
    }

    private void Move(Transform target, Vector3 from, Vector3 to, float duration) {
        StartCoroutine(MoveCoroutine(target, from, to, duration));
    }

    private IEnumerator MoveCoroutine(Transform target, Vector3 from, Vector3 to, float duration) {
        float elapsed = 0f;
        while (elapsed < duration) {
            target.localPosition = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        target.localPosition = to;
    }

    private NumberTileView GetTileView(int num)
    {
        var tileView = freeTileViews.First.Value;
        freeTileViews.RemoveFirst();
        tileView.SetNumber(num);
        tileView.Apply(GetColor(num));
        return tileView;
    }

    private IEnumerator DelayAction(Action action, float delay) {
        yield return new WaitForSeconds(delay);
        action();
    }

    private TileColor GetColor(int num)
    {
        switch (num)
        {
            case 2: return colors[0];
            case 4: return colors[1];
            case 8: return colors[2];
            case 16: return colors[3];
            case 32: return colors[4];
            case 64: return colors[5];
            case 128: return colors[6];
            case 256: return colors[7];
            case 512: return colors[8];
            case 1024: return colors[9];
            case 2048: return colors[10];
        }
        return colors[0];
    }
}
