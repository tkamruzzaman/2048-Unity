using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Node : MonoBehaviour
{
    public Vector3 Position => transform.position;
    public Block OccupiedBlock { get; private set; }
}
