using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{

    private const string KEY_PLAY_SOUNDS = "_key_play_sounds";

    private AudioSource audioSource;

    [SerializeField] private AudioClip[] moveAudioClips;
    [SerializeField] private AudioClip[] mergeAudioClips;
    [SerializeField] private AudioClip[] invalidMoveAudioClips;
    [SerializeField] private AudioClip[] winAudioClips;
    [SerializeField] private AudioClip[] loseAudioClips;

    public bool IsToPlaySounds
    {
        get { return PlayerPrefsBool.GetBool(KEY_PLAY_SOUNDS, true); }
        private set { PlayerPrefsBool.SetBool(KEY_PLAY_SOUNDS, value); }
    }

    private void Awake() => audioSource = GetComponent<AudioSource>();

    private void Start()
    {
        GameManager.Instance.OnAnyBlockMerged += GameManager_OnBlocksMerged;
        GameManager.Instance.OnValidMove += GameManager_OnValidMove;
        GameManager.Instance.OnInvalidMove += GameManager_OnInvalidMove;
        GameManager.Instance.OnGameOver += GameManager_OnGameOver;
    }

    private void GameManager_OnGameOver(object sender, bool e)
    {
        AudioClip audioClip = e
            ? winAudioClips[Random.Range(0, winAudioClips.Length)]
            : loseAudioClips[Random.Range(0, loseAudioClips.Length)];
        PlaySound(audioClip, 0.25f);
    }

    private void GameManager_OnInvalidMove(object sender, EventArgs e)
    {
        PlaySound(invalidMoveAudioClips[Random.Range(0, invalidMoveAudioClips.Length)], 0.15f);
    }

    private void GameManager_OnValidMove(object sender, EventArgs e)
    {
        PlaySound(moveAudioClips[Random.Range(0, moveAudioClips.Length)], 0.1f);
    }

    private void GameManager_OnBlocksMerged(object sender, EventArgs e)
    {
        PlaySound(mergeAudioClips[Random.Range(0, mergeAudioClips.Length)], 0.5f);
    }

    private void PlaySound(AudioClip audioClip, float volumeScale) => audioSource.PlayOneShot(audioClip, volumeScale);

    public void ToggleSound() => IsToPlaySounds = !IsToPlaySounds;

    private void OnDestroy()
    {
        GameManager.Instance.OnAnyBlockMerged -= GameManager_OnBlocksMerged;
        GameManager.Instance.OnValidMove -= GameManager_OnValidMove;
        GameManager.Instance.OnInvalidMove -= GameManager_OnInvalidMove;
        GameManager.Instance.OnGameOver -= GameManager_OnGameOver;
    }

}