using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private BoardView boardView;
    private Board board;
    private GridMap gridMap;

    void Start()
    {
        gridMap = new GridMap(4, 4);
        board = new Board();
        board.Init(gridMap);
        boardView.Init(gridMap);
        boardView.Bind(board);

        board.StartGame();
    }
}
