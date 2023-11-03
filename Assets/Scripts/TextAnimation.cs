using DG.Tweening;
using TMPro;
using UnityEngine;

public class TextAnimation : MonoBehaviour
{
    [SerializeField] private float fadingTime = 1;
    
    private TMP_Text text;

    private void Awake() => text = GetComponentInChildren<TMP_Text>();

    public void Init(int value, Color color)
    {
        text.text = value.ToString();
        text.color = color;

        Sequence sequence = DOTween.Sequence();

        sequence.Insert(0, text.DOFade(0, fadingTime));
        sequence.Insert(0, text.transform.DOMove(text.transform.position + Vector3.up, fadingTime));

        sequence.OnComplete(() => Destroy(gameObject));
    }
}

