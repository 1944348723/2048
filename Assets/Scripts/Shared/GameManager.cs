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
    private KeyboardInputReader inputReader;

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
        inputReader = gameObject.AddComponent<KeyboardInputReader>();

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


        uiScore.SetHighScore(PlayerPrefs.GetInt("HighScore", 0));
        uiScore.SetScore(0);

        // 当前项目所有对象的生命周期都是整个游戏流程，没有销毁的情况，所以只有注册，没有解绑也没问题
        // 当出现UI反复创建销毁之类的涉及生命周期的时候，要考虑进一步管理
        boardView.AnimationFinished += () => { CheckGameOver(); };
        uiGameOver.OnPlayAgain += Restart;

        List<TileAction> actions = board.StartGame();
        boardView.UpdateView(actions);
    }

    void Update()
    {
        if (!inputReader.enabled) return;

        Direction direction = MapDirection(inputReader.GetCurrentDirection());
        if (direction == Direction.None) return;

        bool hasChanged = false;
        List<TileAction> actions = board.TryMove(direction);
        if (actions == null) return;

        hasChanged = actions.Count > 0;
        // 有变化的话暂时禁止输入，等动画结束
        if (hasChanged)
        {
            inputReader.SetActive(false);
        }
        boardView.UpdateView(actions);
        UpdateUI();
    }

    private void Restart()
    {
        boardView.Reset();
        uiGameOver.Reset();
        uiGameOver.gameObject.SetActive(false);
        uiScore.SetScore(0);
        uiScore.SetHighScore(PlayerPrefs.GetInt("HighScore", 0));

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
            inputReader.SetActive(true);
        }
    }

    private void GameOver()
    {
        TryUpdateHighScore();
        uiGameOver.ShowHighScore(PlayerPrefs.GetInt("HighScore", 0));
        uiGameOver.gameObject.SetActive(true);
        Debug.Log("Game Over");
    }

    private void UpdateUI()
    {
        uiScore.SetScore(board.Score);
    }

    private void TryUpdateHighScore()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (board.Score > highScore)
        {
            PlayerPrefs.SetInt("HighScore", board.Score);
            uiScore.SetHighScore(board.Score);
        }
    }

    private Direction MapDirection(InputDirection inputDirection)
    {
        return inputDirection switch
        {
            InputDirection.Up => Direction.Up,
            InputDirection.Down => Direction.Down,
            InputDirection.Left => Direction.Left,
            InputDirection.Right => Direction.Right,
            InputDirection.None => Direction.None,
            _ => throw new ArgumentOutOfRangeException(nameof(inputDirection), $"Unhandled input direction: {inputDirection}")
        };
    }
}
