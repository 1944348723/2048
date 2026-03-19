using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGameOver : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI highScore;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button exitButton;

    public event System.Action OnPlayAgain;

    void OnEnable()
    {
        this.playAgainButton.onClick.AddListener(OnPlayAgainButtonClicked);
        this.exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    void OnDisable()
    {
        this.playAgainButton.onClick.RemoveListener(OnPlayAgainButtonClicked);
        this.exitButton.onClick.RemoveListener(OnExitButtonClicked);
    }

    public void ShowHighScore(int score)
    {
        this.highScore.SetText(score.ToString());
        this.highScore.gameObject.SetActive(true);
        this.highScoreText.gameObject.SetActive(true);
    }

    public void Reset()
    {
        this.highScore.SetText(0.ToString());
        this.highScore.gameObject.SetActive(false);
        this.highScoreText.gameObject.SetActive(false);
    }

    private void OnPlayAgainButtonClicked()
    {
        Debug.Log("OnPlayAgainButtonClicked");
        OnPlayAgain?.Invoke();
    }

    private void OnExitButtonClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
