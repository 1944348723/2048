using System;
using System.Collections.Generic;
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
        if (!boardView)
        {
            Debug.LogWarning("BoardView is not assigned in the inspetor.");
        }
    }

    void Start()
    {
        Test.TestPushLine();

        gridMap = new GridMap(gameConfig.Rows, gameConfig.Columns);
        board = new Board();
        scoreSystem = new ScoreSystem();

        board.Init(gridMap);
        BoardViewParams boardViewParams = new(
            gameConfig.Rows,
            gameConfig.Columns,
            gameConfig.XGap,
            gameConfig.YGap,
            gameConfig.AnimationDuration,
            gameConfig.CellBgPrefab,
            gameConfig.NumberTilePrefab,
            gameConfig.Colors
        );
        boardView.Init(boardViewParams);
        uiScore.SetHighScore(scoreSystem.GetHighScore());

        // 当前项目所有对象的生命周期都是整个游戏流程，没有销毁的情况，所以只有注册，没有解绑也没问题
        // 当出现UI反复创建销毁之类的涉及生命周期的时候，要考虑进一步管理
        boardView.AnimationFinished += () => { CheckGameOver(); };
        scoreSystem.OnScoreChanged += score => { uiScore.SetScore(score); };
        scoreSystem.OnHighScoreChanged += score => { uiGameOver.ShowHighScore(score); };
        uiGameOver.OnPlayAgain += Restart;

        List<TileAction> actions = board.StartGame();
        boardView.UpdateView(actions);
    }

    void Update()
    {
        if (!enableInput) return;

        bool hasChanged = false;
        List<TileAction> actions = null;
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            actions = board.TryMove(Direction.Up);
        } else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            actions = board.TryMove(Direction.Down);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            actions = board.TryMove(Direction.Left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            actions = board.TryMove(Direction.Right);
        }
        if (actions == null) return;

        hasChanged = actions.Count > 0;
        if (hasChanged)
        {
            enableInput = false;
        }
        boardView.UpdateView(actions);
        scoreSystem.SetScore(board.Score);
    }

    private void Restart()
    {
        boardView.Reset();
        scoreSystem.Reset();
        uiGameOver.Reset();
        uiGameOver.gameObject.SetActive(false);
        uiScore.SetHighScore(scoreSystem.GetHighScore());

        var actions = board.StartGame();
        boardView.UpdateView(actions);
    }

    private void CheckGameOver()
    {
        if (board.IsGameOver())
        {
            GameOver();
        } else
        {
            this.enableInput = true;
        }
    }

    private void GameOver()
    {
        scoreSystem.TrySetHighScore();
        uiGameOver.gameObject.SetActive(true);
        Debug.Log("Game Over");
    }
}
