using System;
using System.Collections.Generic;
using UnityEngine;

public readonly struct BoardViewParams
{
    public int Rows { get; }
    public int Cols { get; }
    public int XGap { get; }
    public int YGap { get; }
    public float MoveAnimationDuration { get; }
    public GameObject CellBgPrefab { get; }
    public GameObject NumberTilePrefab { get; }
    public TileColor[] Colors { get; }

    public BoardViewParams(
        int rows,
        int cols,
        int xGap,
        int yGap,
        float moveAnimationDuration,
        GameObject cellBgPrefab,
        GameObject numberTilePrefab,
        TileColor[] colors)
    {
        if (rows < 1) throw new ArgumentOutOfRangeException(nameof(rows), "rows must be greater than 0.");
        if (cols < 1) throw new ArgumentOutOfRangeException(nameof(cols), "cols must be greater than 0.");
        if (xGap < 0) throw new ArgumentOutOfRangeException(nameof(xGap), "xGap must be greater than or equal to 0.");
        if (yGap < 0) throw new ArgumentOutOfRangeException(nameof(yGap), "yGap must be greater than or equal to 0.");
        if (moveAnimationDuration < 0) throw new ArgumentOutOfRangeException(nameof(moveAnimationDuration), "moveAnimationDuration must be greater than or equal to 0.");
        if (!cellBgPrefab) throw new ArgumentNullException(nameof(cellBgPrefab), "cellBgPrefab is required.");
        if (!numberTilePrefab) throw new ArgumentNullException(nameof(numberTilePrefab), "numberTilePrefab is required.");
        if (colors == null || colors.Length == 0) throw new ArgumentException("colors must not be null or empty.", nameof(colors));

        Rows = rows;
        Cols = cols;
        XGap = xGap;
        YGap = yGap;
        MoveAnimationDuration = moveAnimationDuration;
        CellBgPrefab = cellBgPrefab;
        NumberTilePrefab = numberTilePrefab;
        Colors = colors;
    }
}

public sealed class BoardView : MonoBehaviour
{
    private int rows;
    private int cols;
    private Vector2 gap = new();
    private TileColor[] colors;
    private GameObject tileBgPrefab;
    private GameObject numberTilePrefab;
    private float moveAnimationDuration;
    private bool isInitialized;

    public event System.Action AnimationFinished;

    private GameObject tileBgContainer;
    private GameObject numberTileContainer;
    private Vector2 cellSize = new();
    private Vector2 leftTop;
    private LinkedList<NumberTileView> freeTileViews;
    private NumberTileView[,] activeTileViews;
    private int tweenCount = 0;

    public void Init(BoardViewParams initParams)
    {
        if (isInitialized)
        {
            throw new InvalidOperationException("BoardView has already been initialized.");
        }

        ApplyInitParams(initParams);
        CreateContainers();
        ConfigureLayout();
        GenerateTileBgs();
        PreCreateTileViews();
        isInitialized = true;
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
    
    public void UpdateView(List<TileAction> actions)
    {
        foreach (var action in actions)
        {
            switch (action.ActionType)
            {
                case TileActionType.Spawn:
                    {
                        var tileView = GetTileView(action.Val);
                        tileView.transform.localPosition = GridToPosition(action.To.x, action.To.y);
                        activeTileViews[action.To.x, action.To.y] = tileView;
                        tileView.gameObject.SetActive(true);
                    }
                    break;
                case TileActionType.Move:
                    {
                        var tileView = activeTileViews[action.From1.x, action.From1.y];
                        activeTileViews[action.From1.x, action.From1.y] = null;
                        activeTileViews[action.To.x, action.To.y] = tileView;

                        Move(tileView.transform, GridToPosition(action.To.x, action.To.y), moveAnimationDuration);
                    }
                    break;
                case TileActionType.Merge:
                    {
                        // 移动并消失
                        var tileView1 = activeTileViews[action.From1.x, action.From1.y];
                        var tileView2 = activeTileViews[action.From2.x, action.From2.y];
                        activeTileViews[action.From1.x, action.From1.y] = null;
                        activeTileViews[action.From2.x, action.From2.y] = null;
                        Move(tileView1.transform, GridToPosition(action.To.x, action.To.y), moveAnimationDuration);
                        Move(tileView2.transform, GridToPosition(action.To.x, action.To.y), moveAnimationDuration);
                        var tween = DOVirtual.DelayedCall(moveAnimationDuration, () => {
                            tileView1.gameObject.SetActive(false);
                            tileView2.gameObject.SetActive(false);
                            freeTileViews.AddFirst(tileView1);
                            freeTileViews.AddFirst(tileView2);
                        });
                        Track(tween);

                        // 延迟出现新tile
                        var newtileView = GetTileView(action.Val);
                        activeTileViews[action.To.x, action.To.y] = newtileView;
                        newtileView.transform.localPosition = GridToPosition(action.To.x, action.To.y);
                        var tween2 = DOVirtual.DelayedCall(moveAnimationDuration, () => {
                            newtileView.gameObject.SetActive(true);
                        });
                        Track(tween2);
                    }
                    break;
            }
        }
    }

    private void ApplyInitParams(BoardViewParams initParams)
    {
        this.rows = initParams.Rows;
        this.cols = initParams.Cols;
        this.gap.x = initParams.XGap;
        this.gap.y = initParams.YGap;
        this.moveAnimationDuration = initParams.MoveAnimationDuration;
        this.tileBgPrefab = initParams.CellBgPrefab;
        this.numberTilePrefab = initParams.NumberTilePrefab;
        this.colors = initParams.Colors;
    }

    private void CreateContainers()
    {
        tileBgContainer = new GameObject("TileBgContainer");
        tileBgContainer.transform.SetParent(transform, false);
        numberTileContainer = new GameObject("NumberTileContainer");
        numberTileContainer.transform.SetParent(transform, false);
    }

    private void ConfigureLayout()
    {
        cellSize.x = tileBgPrefab.GetComponent<RectTransform>().rect.width;
        cellSize.y = tileBgPrefab.GetComponent<RectTransform>().rect.height;
        this.leftTop = new Vector2(
            (-cols / 2.0f + 0.5f) * cellSize.x - (cols - 1) / 2f * gap.x,
            (rows / 2.0f - 0.5f) * cellSize.y + (rows - 1) / 2f * gap.y
        );
    }

    private void GenerateTileBgs()
    {
        for (int r = 0; r < rows; ++r)
        {
            for (int c = 0; c < cols; ++c)
            {
                GameObject tileBg = Instantiate(tileBgPrefab);
                tileBg.transform.SetParent(tileBgContainer.transform);
                tileBg.transform.localPosition = GridToPosition(r, c);
            }
        }
    }

    private Vector2 GridToPosition(int row, int col)
    {
        return new Vector2(this.leftTop.x + col * cellSize.x + col * gap.x, this.leftTop.y - row * cellSize.y - row * gap.y);
    }


    private void PreCreateTileViews()
    {
        freeTileViews = new LinkedList<NumberTileView>();
        activeTileViews = new NumberTileView[rows, cols];
        int count = rows * cols;
        // 多创建几个，假设棋盘摆满2，这时候移动的话，合并需要额外8个，生成需要额外1个，所以最极端情况下需要额外9个
        for (int i = 0; i < count + 9; ++i)
        {
            GameObject tile = Instantiate(numberTilePrefab);
            tile.transform.SetParent(numberTileContainer.transform);
            NumberTileView tileComponent = tile.GetComponent<NumberTileView>();
            freeTileViews.AddFirst(tileComponent);
            tile.SetActive(false);
        }
    }

    private void Move(Transform target, Vector3 to, float duration) {
        var tween = DoTween.To(
            () => target.localPosition,
            pos => target.localPosition = pos,
            to,
            duration
        );
        Track(tween);
    }

    private void Track(TweenBase tween) {
        ++tweenCount;
        tween.OnComplete(FinishOne);
    }

    private void FinishOne() {
        --tweenCount;
        if (tweenCount == 0) {
            AnimationFinished?.Invoke();
        }
    }

    private NumberTileView GetTileView(int num)
    {
        var tileView = freeTileViews.First.Value;
        freeTileViews.RemoveFirst();
        tileView.SetNumber(num);
        tileView.Apply(GetColor(num));
        return tileView;
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
