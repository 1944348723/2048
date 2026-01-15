using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float animationDuration = 0.1f;
    [SerializeField] private BoardView boardView;
    [SerializeField] private UIScore uiScore;
    [SerializeField] private UIGameOver uiGameOver;

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
            hasChanged = board.Push(Direction.Up, true);
        } else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            hasChanged = board.Push(Direction.Down, true);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            hasChanged = board.Push(Direction.Left, true);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            hasChanged = board.Push(Direction.Right, true);
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
        if (board.Push(Direction.Up, false)
            || board.Push(Direction.Down, false)
            || board.Push(Direction.Left, false)
            || board.Push(Direction.Right, false))
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
