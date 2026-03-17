using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private BoardView boardView;
    [SerializeField] private UIScore uiScore;
    [SerializeField] private UIGameOver uiGameOver;

    private GridMap gridMap;
    private Board board;
    private ScoreSystem scoreSystem;

    private bool enableInput = true;

    private void Awake()
    {
        if (!gameConfig)
        {
            throw new ArgumentNullException(nameof(gameConfig), "GameConfig is not assigned in the inspector.");
        }
    }

    void Start()
    {
        Test.TestPushLine();

        gridMap = new GridMap(gameConfig.Rows, gameConfig.Columns);
        board = new Board();
        scoreSystem = new ScoreSystem();

        board.Init(gridMap);
        boardView.Init(gameConfig);
        boardView.SetAnimationDuration(gameConfig.AnimationDuration);
        uiScore.SetHighScore(scoreSystem.GetHighScore());

        // 当前项目所有对象的生命周期都是整个游戏流程，没有销毁的情况，所以只有注册，没有解绑也没问题
        // 当出现UI反复创建销毁之类的涉及生命周期的时候，要考虑进一步管理
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
