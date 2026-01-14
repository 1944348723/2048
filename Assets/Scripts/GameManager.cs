using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private BoardView boardView;
    [SerializeField] private float animationDuration = 0.1f;

    private Board board;
    private GridMap gridMap;
    private bool enableInput = true;

    void Start()
    {
        Test.TestPushLine();

        gridMap = new GridMap(4, 4);
        board = new Board();
        board.Init(gridMap);
        boardView.Init(gridMap);
        boardView.Bind(board);
        boardView.SetAnimationDuration(this.animationDuration);
        boardView.OnAnimationFinished += () => { this.enableInput = true; };

        board.StartGame();
    }

    void Update()
    {
        if (!enableInput) return;

        bool hasChanged = false;
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            hasChanged = board.Push(Direction.Up);
        } else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            hasChanged = board.Push(Direction.Down);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            hasChanged = board.Push(Direction.Left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            hasChanged = board.Push(Direction.Right);
        }
        if (hasChanged)
        {
            enableInput = false;
        }
    }
}
