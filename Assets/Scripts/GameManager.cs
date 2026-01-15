using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float animationDuration = 0.1f;
    [SerializeField] private BoardView boardView;
    [SerializeField] private UIScore uiScore;
    [SerializeField] private UIGameOver uiGameOver;

    private const int Rows = 4;
    private const int Cols = 4;

    private GridMap gridMap;
    private Board board;
    private ScoreSystem scoreSystem;

    private bool enableInput = true;

    void Start()
    {
        Test.TestPushLine();

        gridMap = new GridMap(Rows, Cols);
        board = new Board();
        scoreSystem = new ScoreSystem();

        board.Init(gridMap);
        boardView.Init(Rows, Cols);
        boardView.SetAnimationDuration(this.animationDuration);
        uiScore.SetHighScore(scoreSystem.GetHighScore());

        boardView.Bind(board);
        boardView.OnAnimationFinished += () => { CheckGameOver(); };
        board.OnMerge += newVal => { scoreSystem.AddScore(newVal); };
        scoreSystem.OnScoreChanged += score => { uiScore.SetScore(score); };
        scoreSystem.OnHighScoreChanged += score => { uiGameOver.ShowHighScore(score); };
        uiGameOver.OnPlayAgain += Restart;

        board.StartGame();
    }

    void Update()
    {
        if (!enableInput) return;

        bool hasChanged = false;
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            hasChanged = board.TryMove(Direction.Up);
        } else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            hasChanged = board.TryMove(Direction.Down);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            hasChanged = board.TryMove(Direction.Left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            hasChanged = board.TryMove(Direction.Right);
        }
        if (hasChanged)
        {
            enableInput = false;
        }
    }

    private void Restart()
    {
        boardView.Reset();
        board.StartGame();
        scoreSystem.Reset();
        uiGameOver.Reset();
        uiGameOver.gameObject.SetActive(false);
        uiScore.SetHighScore(scoreSystem.GetHighScore());
    }

    private void CheckGameOver()
    {
        if (board.CanMove(Direction.Up)
            || board.CanMove(Direction.Down)
            || board.CanMove(Direction.Left)
            || board.CanMove(Direction.Right))
        {
            this.enableInput = true;
        } else
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        scoreSystem.TrySetHighScore();
        uiGameOver.gameObject.SetActive(true);
        Debug.Log("Game Over");
    }
}
