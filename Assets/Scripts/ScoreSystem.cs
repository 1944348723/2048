
using UnityEngine;

public class ScoreSystem
{
    public event System.Action<int> OnScoreChanged;
    private int score = 0;
    private string highScoreKey = "HighScore";

    public void AddScore(int increment)
    {
        this.score += increment;
        this.OnScoreChanged?.Invoke(this.score);
    }

    public int GetHighScore()
    {
        return PlayerPrefs.GetInt(highScoreKey, 0);
    }

    public bool TrySetHighScore()
    {
        int highScore = PlayerPrefs.GetInt(highScoreKey, 0);
        if (this.score <= highScore) return false;

        PlayerPrefs.SetInt(highScoreKey, this.score);
        return true;
    }
}
