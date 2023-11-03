using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Board : MonoBehaviour
{
    public   int Width{get; private set;}
    public  int Height { get; private set; }
    public Vector2 Center { get; private set; }

    private SpriteRenderer spriteRenderer;

    private Dictionary<GridPosition, Node> nodeDictionary;

    private void Awake()
    {
        Width = 4;
        Height = 4;
        Center = new Vector3(Width / 2, Height / 2, 0);

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        nodeDictionary = new();
    }

    public void Init()
    {
        SetPosition();
        SetSize();
        PlaceCamera();
        nodeDictionary.Clear();
    }

    private void SetPosition() => transform.position = Center;

    private void SetSize() => spriteRenderer.size = new Vector2(Width, Height);

    private void PlaceCamera() => Camera.main.transform.position = new Vector3(Center.x, Center.y, -10f);

    public void AddNodeToGridPosition(GridPosition gridPosition, Node node) 
        => nodeDictionary.Add(gridPosition, node);

    public Node GetNodeAtGridPosition(GridPosition gridPosition) 
        => nodeDictionary.TryGetValue(gridPosition, out Node node) ? node : null;

    public bool IsValidGridPosition(GridPosition gridPosition)
    => gridPosition.x >= 0
        && gridPosition.z >= 0
        && gridPosition.x < Width
        && gridPosition.z < Height;
}
