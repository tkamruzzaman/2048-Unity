using DG.Tweening;
using System;
using UnityEngine;

public class Button3D : MonoBehaviour
{
    public Action OnButtonPressed = delegate { };

    private Vector3 scale;

    private void Awake() => scale = transform.localScale;

    private void OnMouseDown() => transform.DOScale(scale * 0.9f, 0.2f);

    private void OnMouseUpAsButton() => transform.DOScale(scale, 0.2f);

    private void OnMouseUp() => OnButtonPressed?.Invoke();

}