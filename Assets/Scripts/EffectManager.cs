using UnityEngine;

public class EffectManager : MonoBehaviour
{
    private const string KEY_SHOW_EFFECTS = "_key_show_effects";

    [SerializeField] private TextAnimation textAnimationPrefab;
    [SerializeField] private ParticleSystem mergeEffectPrefab;
    [Space]
    [SerializeField] private Transform effectsParentTransform;
    public bool IsToShowEffects
    {
        get { return PlayerPrefsBool.GetBool(KEY_SHOW_EFFECTS, true); }
        private set { PlayerPrefsBool.SetBool(KEY_SHOW_EFFECTS, value); }
    }

    private void Start()
    {
        GameManager.Instance.OnBlocksMerged += GameManager_OnBlocksMerged;
    }

    private void GameManager_OnBlocksMerged(object sender, GameManager.OnBlocksMergedEventArgs e)
    {
        ShowEffects(e.BlockA, e.newValue);
    }

    private void ShowEffects(Block baseBlock, int newValue)
    {
        if (IsToShowEffects)
        {
            ParticleSystem particleSystem = Instantiate(mergeEffectPrefab, baseBlock.Position, Quaternion.identity, effectsParentTransform);
            //ParticleSystem.MainModule mainModule = particleSystem.main;
            //mainModule.startColor = GetBlockTypeByValue(newValue).BlockColor;
            TextAnimation textAnimation = Instantiate(textAnimationPrefab, baseBlock.Position, Quaternion.identity, effectsParentTransform);
            textAnimation.Init(newValue, GameManager.Instance.GetBlockTypeByValue(newValue).TextColor);
        }
    }

    public void ToggleEffect() => IsToShowEffects = !IsToShowEffects;

    private void OnDestroy()
    {
        GameManager.Instance.OnBlocksMerged -= GameManager_OnBlocksMerged;
    }

}
