using System;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// 集成测试
/// 我懒得写了，这个是AI生成的，但是我看过并跑过了，还发现最后一个测试一开始写错了，AI只考虑到了合并和生成，把移动给漏掉了
/// 且只是很简单的测试了下
/// </summary>
public class BoardTests
{
    [Test]
    public void Constructor_WhenGridMapIsNull_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() => _ = new Board(null));
    }

    [Test]
    public void IsGameOver_WhenGridIsNotFull_ShouldReturnFalse()
    {
        var gridMap = new GridMap(4, 4);
        gridMap.Fill(2);
        gridMap.Set(0, 0, 0);
        var board = new Board(gridMap);

        Assert.That(board.IsGameOver(), Is.False);
    }

    [Test]
    public void IsGameOver_WhenGridIsFullAndHasMerge_ShouldReturnFalse()
    {
        var gridMap = new GridMap(4, 4);
        FillGrid(gridMap, new[,]
        {
            { 2, 2, 4, 8 },
            { 16, 32, 64, 128 },
            { 256, 512, 1024, 2 },
            { 4, 8, 16, 32 }
        });
        var board = new Board(gridMap);

        Assert.That(board.IsGameOver(), Is.False);
    }

    [Test]
    public void IsGameOver_WhenGridIsFullAndNoMerge_ShouldReturnTrue()
    {
        var gridMap = new GridMap(4, 4);
        FillGrid(gridMap, new[,]
        {
            { 2, 4, 2, 4 },
            { 4, 2, 4, 2 },
            { 2, 4, 2, 4 },
            { 4, 2, 4, 2 }
        });
        var board = new Board(gridMap);

        Assert.That(board.IsGameOver(), Is.True);
    }

    [Test]
    public void TryMove_WhenNoTileChanges_ShouldReturnNoActionAndNoScoreChange()
    {
        var gridMap = new GridMap(4, 4);
        FillGrid(gridMap, new[,]
        {
            { 2, 4, 8, 16 },
            { 32, 64, 128, 256 },
            { 512, 1024, 2, 4 },
            { 8, 16, 32, 64 }
        });
        var board = new Board(gridMap);

        var actions = board.TryMove(Direction.Left);

        Assert.That(actions, Is.Empty);
        Assert.That(board.Score, Is.EqualTo(0));
        Assert.That(gridMap.Data(), Is.EqualTo(new[,]
        {
            { 2, 4, 8, 16 },
            { 32, 64, 128, 256 },
            { 512, 1024, 2, 4 },
            { 8, 16, 32, 64 }
        }));
    }

    [Test]
    public void TryMove_WhenLeftMergeHappens_ShouldIncreaseScoreAndAppendSpawn()
    {
        var gridMap = new GridMap(4, 4);
        FillGrid(gridMap, new[,]
        {
            { 2, 2, 4, 8 },
            { 16, 32, 64, 128 },
            { 256, 512, 1024, 2 },
            { 4, 8, 16, 32 }
        });
        var board = new Board(gridMap);

        var actions = board.TryMove(Direction.Left);

        Assert.That(board.Score, Is.EqualTo(4));
        Assert.That(actions.Count, Is.EqualTo(4));

        Assert.That(actions[0].ActionType, Is.EqualTo(TileActionType.Merge));
        Assert.That(actions[0].From1, Is.EqualTo(new Vector2Int(0, 0)));
        Assert.That(actions[0].From2, Is.EqualTo(new Vector2Int(0, 1)));
        Assert.That(actions[0].To, Is.EqualTo(new Vector2Int(0, 0)));
        Assert.That(actions[0].Val, Is.EqualTo(4));

        Assert.That(actions[1].ActionType, Is.EqualTo(TileActionType.Move));
        Assert.That(actions[1].From1, Is.EqualTo(new Vector2Int(0, 2)));
        Assert.That(actions[1].To, Is.EqualTo(new Vector2Int(0, 1)));

        Assert.That(actions[2].ActionType, Is.EqualTo(TileActionType.Move));
        Assert.That(actions[2].From1, Is.EqualTo(new Vector2Int(0, 3)));
        Assert.That(actions[2].To, Is.EqualTo(new Vector2Int(0, 2)));

        var spawnAction = actions.Last();
        Assert.That(spawnAction.ActionType, Is.EqualTo(TileActionType.Spawn));
        Assert.That(spawnAction.To, Is.EqualTo(new Vector2Int(0, 3)));
        Assert.That(spawnAction.Val == 2 || spawnAction.Val == 4, Is.True);
        Assert.That(gridMap.Get(0, 3), Is.EqualTo(spawnAction.Val));
    }

    private static void FillGrid(GridMap gridMap, int[,] values)
    {
        for (int r = 0; r < gridMap.Rows; ++r)
        {
            for (int c = 0; c < gridMap.Cols; ++c)
            {
                gridMap.Set(r, c, values[r, c]);
            }
        }
    }
}
