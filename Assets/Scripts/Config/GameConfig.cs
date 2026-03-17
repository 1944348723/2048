using UnityEngine;

[CreateAssetMenu(menuName = "GameConfig")]
public sealed class GameConfig : ScriptableObject
{
    [SerializeField] private int rows = 4;
    [SerializeField] private int columns = 4;
    [SerializeField] private int xGap = 20;
    [SerializeField] private int yGap = 20;
    [SerializeField] private float animationDuration = 0.1f;
    [SerializeField] private GameObject cellBgPrefab;
    [SerializeField] private GameObject numberTilePrefab;

    public int Rows => rows;
    public int Columns => columns;
    public int XGap => xGap;
    public int YGap => yGap;
    public float AnimationDuration => animationDuration;
    public GameObject CellBgPrefab => cellBgPrefab;
    public GameObject NumberTilePrefab => numberTilePrefab;

    private void OnValidate()
    {
        if (rows < 1) rows = 1;
        if (columns < 1) columns = 1;
        if (animationDuration < 0) animationDuration = 0;
    } 
}
