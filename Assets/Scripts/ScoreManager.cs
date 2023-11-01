using System;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreManager : MonoBehaviour
{
    public event EventHandler OnScoreChanged;

    private const string KEY_PREFS_BEST_SCORE = "_key_prefs_best_score";

    public int CurrentScore { get; private set; }
    public int BestScore { get; private set; }

    private void Awake() => BestScore = PlayerPrefs.GetInt(KEY_PREFS_BEST_SCORE, 0);

    private void Start()
    {
        GameManager.Instance.OnBlocksMerged += GameManager_OnBlocksMerged;
        GameManager.Instance.OnGameReload += GameManager_OnGameReload;
    }

    private void GameManager_OnBlocksMerged(object sender, GameManager.OnBlocksMergedEventArgs e) => AddScore(e.newValue);

    private void GameManager_OnGameReload(object sender, EventArgs e) => SetScore(0);

    private void OnDestroy()
    {
        GameManager.Instance.OnBlocksMerged -= GameManager_OnBlocksMerged;
        GameManager.Instance.OnGameReload -= GameManager_OnGameReload;
    }

    private void SetScore(int score)
    {
        CurrentScore = score;

        OnScoreChanged?.Invoke(this, EventArgs.Empty);
    }

    private void AddScore(int score)
    {
        CurrentScore += score;

        TryToSetBestScore();

        OnScoreChanged?.Invoke(this, EventArgs.Empty);
    }

    private void TryToSetBestScore()
    {
        if (CurrentScore > BestScore)
        {
            BestScore = CurrentScore;
            PlayerPrefs.SetInt(KEY_PREFS_BEST_SCORE, BestScore);
        }
    }

}
