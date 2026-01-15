using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float animationDuration = 0.1f;
    [SerializeField] private BoardView boardView;
    [SerializeField] private UIScore uiScore;

    private GridMap gridMap;
    private Board board;
    private ScoreSystem scoreSystem;

    private bool enableInput = true;

    void Start()
    {
        Test.TestPushLine();

        gridMap = new GridMap(4, 4);
        board = new Board();
        scoreSystem = new ScoreSystem();

        board.Init(gridMap);
        boardView.Init(gridMap);
        boardView.SetAnimationDuration(this.animationDuration);
        uiScore.SetHighScore(scoreSystem.GetHighScore());

        boardView.Bind(board);
        boardView.OnAnimationFinished += () => { this.enableInput = true; };
        board.OnMerge += newVal => { scoreSystem.AddScore(newVal); };
        scoreSystem.OnScoreChanged += score => { uiScore.SetScore(score); };

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
