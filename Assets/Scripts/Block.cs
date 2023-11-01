using TMPro;
using UnityEngine;

[SelectionBase]
public class Block : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private TMP_Text text;

    public Vector2 Position => transform.position;
    public BlockType BlockType { get; private set; }
    public int Value { get; private set; }
    public Color BlockColor { get; private set; }
    public Color TextColor { get; private set; }
    public Node OccupiedNode { get; private set; }
    public Block BlockToMergeWith { get; private set; }
    public bool IsMerging { get; private set; }

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        text = GetComponentInChildren<TMP_Text>();
    }

    public void Init(BlockType blockType)
    {
        BlockType = blockType;
        Value = blockType.Value;
        BlockColor = blockType.BlockColor;
        TextColor = blockType.TextColor;

        spriteRenderer.color = BlockColor;
        text.text = Value.ToString();
        text.color = TextColor;
    }

    public void SetBlock(Node node)
    {
        if (OccupiedNode != null)
        {
            OccupiedNode.SetOccupingBlock(null);
        }

        OccupiedNode = node;
        OccupiedNode.SetOccupingBlock(this);
    }

    public void MergeBlock(Block blockToMergeWith)
    {
        //set the block we are merging with
        BlockToMergeWith = blockToMergeWith;
        //set current node as unoccupied, to allow other blocks to use it
        OccupiedNode.SetOccupingBlock(null);
        //set the base block as merging, so it does not get used more than once
        blockToMergeWith.IsMerging = true;
    }

    public bool CanMerge(int value) => value == Value && !IsMerging && BlockToMergeWith == null;

}
