using System;
using UnityEngine;

[Serializable]
public struct BlockType
{
    [field: SerializeField]
    public int Value { get; private set; }

    [field: SerializeField]
    public Color Color { get; private set; }
}