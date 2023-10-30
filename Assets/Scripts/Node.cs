//#define TESTING
using UnityEngine;

[SelectionBase]
public class Node : MonoBehaviour
{
    public Vector2 Position => transform.position;
    public Block OccupingBlock { get; private set; }

    public void SetOccupingBlock(Block block)
    {
        OccupingBlock = block;
    }

#if TESTING
    private void Update()
    {
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (OccupingBlock == null) { spriteRenderer.color = Color.white; }
        else { spriteRenderer.color = Color.gray; }
    }
#endif

}
