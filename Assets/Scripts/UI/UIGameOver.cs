using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGameOver : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI highScore;
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button exitButton;

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

    public void SetHighScore(int score)
    {
        this.highScore.SetText(score.ToString());
    }

    private void OnPlayAgainButtonClicked()
    {
        Debug.Log("OnPlayAgainButtonClicked");
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
