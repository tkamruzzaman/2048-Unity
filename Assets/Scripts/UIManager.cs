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
    [Space]
    [SerializeField] private Button3D effectsToggle3DButton;
    [SerializeField] private SpriteRenderer effectsToggleSpriteRenderer;
    [Space]
    [SerializeField] private Button3D soundsToggle3DButton;
    [SerializeField] private SpriteRenderer soundsToggleSpriteRenderer;
    [Space]
    [SerializeField] private Sprite toggleOnSprite;
    [SerializeField] private Sprite toggleOffSprite;

    private ScoreManager scoreManager;
    private EffectManager effectManager;
    private SoundManager soundManager;

    private void Start()
    {
        effectManager = FindObjectOfType<EffectManager>(); if (effectManager == null) { Debug.Log($"EffectManager is : {effectManager}"); }
        soundManager = FindObjectOfType<SoundManager>(); if (soundManager == null) { Debug.Log($"SoundManager is : {soundManager}"); }
        scoreManager = FindObjectOfType<ScoreManager>(); if (scoreManager == null) { Debug.Log($"ScoreManager is : {scoreManager}"); }

        scoreManager.OnScoreChanged += ScoreManager_OnScoreChanged;
        GameManager.Instance.OnGameOver += GameManager_OnGameOver;
        GameManager.Instance.OnGameReload += GameManager_OnGameReload;

        newGame3DButton.OnButtonPressed += NewGameButtonPressedAction;
        effectsToggle3DButton.OnButtonPressed += EffectToggleButtonPressedAction;
        soundsToggle3DButton.OnButtonPressed += SoundToggleButtonPressedAction;

        Initialization();
    }

    private void Initialization()
    {
        gameOverUITransform.gameObject.SetActive(false);

        UpdateTexts();
        UpdateEffectToggle();
        UpdateSoundToggle();
    }

    private void GameManager_OnGameReload(object sender, EventArgs e) => Initialization();

    private void ScoreManager_OnScoreChanged(object sender, EventArgs e) => UpdateTexts();

    private void GameManager_OnGameOver(object sender, bool result)
    {
        gameOverText.text = result ? "You Win!" : "You Lose!";
        gameOverUITransform.gameObject.SetActive(true);
    }

    private void NewGameButtonPressedAction() => GameManager.Instance.ReloadLevel();

    private void EffectToggleButtonPressedAction() => UpdateEffectToggle(true);

    private void SoundToggleButtonPressedAction() => UpdateSoundToggle(true);

    private void UpdateTexts()
    {
        scoreText.text = scoreManager.CurrentScore.ToString();
        bestScoreText.text = scoreManager.BestScore.ToString();
    }

    private void UpdateEffectToggle(bool isToUpdate = false)
    {
        if (isToUpdate) { effectManager.ToggleEffect(); }

        if (effectManager.IsToShowEffects)
        {
            effectsToggleSpriteRenderer.sprite = toggleOnSprite;
        }
        else
        {
            effectsToggleSpriteRenderer.sprite = toggleOffSprite;
        }
    }

    private void UpdateSoundToggle(bool isToUpdate = false)
    {
        if (isToUpdate) { soundManager.ToggleSound(); }

        if(soundManager.IsToPlaySounds)
        {
            soundsToggleSpriteRenderer.sprite = toggleOnSprite;
        }
        else
        {
            soundsToggleSpriteRenderer.sprite = toggleOffSprite;
        }
    }

    private void OnDestroy()
    {
        scoreManager.OnScoreChanged -= ScoreManager_OnScoreChanged;
        GameManager.Instance.OnGameOver -= GameManager_OnGameOver;
        GameManager.Instance.OnGameReload -= GameManager_OnGameReload;

        newGame3DButton.OnButtonPressed -= NewGameButtonPressedAction;
        effectsToggle3DButton.OnButtonPressed -= EffectToggleButtonPressedAction;
        soundsToggle3DButton.OnButtonPressed -= SoundToggleButtonPressedAction;

    }
}
