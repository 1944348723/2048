using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private BoardView boardView;
    private Board board;
    private GridMap gridMap;

    void Start()
    {
        Test.TestPushLine();

        gridMap = new GridMap(4, 4);
        board = new Board();
        board.Init(gridMap);
        boardView.Init(gridMap);
        boardView.Bind(board);

        board.StartGame();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            board.Push(Direction.Up);
        } else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            board.Push(Direction.Down);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            board.Push(Direction.Left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            board.Push(Direction.Right);
        }
    }
}
