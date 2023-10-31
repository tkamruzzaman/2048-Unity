using UnityEngine;

public class Board : MonoBehaviour
{
    public   int Width{get; private set;}
    public  int Height { get; private set; }
    public Vector2 Center { get; private set; }

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        Width = 4;
        Height = 4;
        Center = new Vector3(Width / 2, Height / 2, 0);

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void Init()
    {
        SetPosition();
        SetSize();
        PlaceCamera();
    }

    private void SetPosition()=> transform.position = Center;

    private void SetSize() => spriteRenderer.size = new Vector2(Width, Height);

    private void PlaceCamera() => Camera.main.transform.position = new Vector3(Center.x, Center.y, -10f);
}
