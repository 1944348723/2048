
using UnityEngine;

public class ScoreSystem
{
    public event System.Action<int> OnScoreChanged;
    public event System.Action<int> OnHighScoreChanged;
    private int score = 0;
    private const string HighScoreKey = "HighScore";

    public void AddScore(int increment)
    {
        this.score += increment;
        this.OnScoreChanged?.Invoke(this.score);
    }

    public int GetHighScore()
    {
        return PlayerPrefs.GetInt(HighScoreKey, 0);
    }

    public bool TrySetHighScore()
    {
        int highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        if (this.score <= highScore) return false;

        PlayerPrefs.SetInt(HighScoreKey, this.score);
        OnHighScoreChanged?.Invoke(this.score);
        return true;
    }

    public void Reset()
    {
        this.score = 0;
        this.OnScoreChanged?.Invoke(0);
    }
}
