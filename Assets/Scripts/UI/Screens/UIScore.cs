using TMPro;
using UnityEngine;

public class UIScore : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI highScoreText;

    public void SetScore(int score)
    {
        this.scoreText.SetText(score.ToString());
    }

    public void SetHighScore(int highScore)
    {
        this.highScoreText.SetText(highScore.ToString());
    }
}
