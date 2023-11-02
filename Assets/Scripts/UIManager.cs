using System;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text bestScoreText;
    [SerializeField] private Button3D newGame3DButton;
    [SerializeField] private Transform gameOverUITransform;
    [SerializeField] private TMP_Text gameOverText;

    private ScoreManager scoreManager;

    private void OnEnable() => newGame3DButton.OnButtonPressed += ButtonPressedAction;

    private void OnDisable() => newGame3DButton.OnButtonPressed -= ButtonPressedAction;

    private void Start()
    {
        gameOverUITransform.gameObject.SetActive(false);

        scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager == null)
        {
            Debug.Log($"ScoreManager is : {scoreManager}");
        }
        scoreManager.OnScoreChanged += ScoreManager_OnScoreChanged;
        GameManager.Instance.OnGameOver += GameManager_OnGameOver;

        UpdateTexts();
    }

    private void OnDestroy()
    {
        scoreManager.OnScoreChanged -= ScoreManager_OnScoreChanged;
        GameManager.Instance.OnGameOver -= GameManager_OnGameOver;

    }

    private void ScoreManager_OnScoreChanged(object sender, EventArgs e) => UpdateTexts();

    private void GameManager_OnGameOver(object sender, bool result)
    {
        gameOverText.text = result ? "You Win!" : "You Lose!";
        gameOverUITransform.gameObject.SetActive(true);
    }

    private void ButtonPressedAction() => GameManager.Instance.ReloadLevel();

    private void UpdateTexts()
    {
        scoreText.text = scoreManager.CurrentScore.ToString();
        bestScoreText.text = scoreManager.BestScore.ToString();
    }
}
