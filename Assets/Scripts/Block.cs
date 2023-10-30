using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[SelectionBase]
public class Block : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private TMP_Text text;

    private BlockType blockType;
    private int value;
    private Color color;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        text = GetComponentInChildren<TMP_Text>();
    }

    public void Init(BlockType blockType)
    {
        this.blockType = blockType;
        value = blockType.Value;
        color = blockType.Color;

        spriteRenderer.color = color;
        text.text = value.ToString();
    }
}
