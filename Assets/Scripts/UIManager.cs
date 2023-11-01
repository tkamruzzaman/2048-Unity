using System;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text bestScoreText;
    [SerializeField] private Button3D newGame3DButton;

    private ScoreManager scoreManager;

    private void OnEnable() => newGame3DButton.OnButtonPressed += ButtonPressedAction;

    private void OnDisable() => newGame3DButton.OnButtonPressed -= ButtonPressedAction;

    private void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager == null)
        {
            Debug.Log($"ScoreManager is : {scoreManager}");
        }
        scoreManager.OnScoreChanged += ScoreManager_OnScoreChanged;
        UpdateTexts();
    }

    private void OnDestroy()
    {
        scoreManager.OnScoreChanged -= ScoreManager_OnScoreChanged;
    }

    private void ScoreManager_OnScoreChanged(object sender, EventArgs e) => UpdateTexts();

    private void ButtonPressedAction() => GameManager.Instance.ReloadLevel();

    private void UpdateTexts()
    {
        scoreText.text = scoreManager.CurrentScore.ToString();
        bestScoreText.text = scoreManager.BestScore.ToString();
    }
}
