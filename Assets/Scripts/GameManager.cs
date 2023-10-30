using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer boardPrefab;
    [SerializeField] private Node nodePrefab;
    [SerializeField] private Block blockPrefab;
    [Space]
    [SerializeField] private Transform boardTransform;
    [Space]
    [SerializeField] private List<BlockType> blockTypeList;

    private const int width = 4;
    private const int height = 4;
    private const int winCondition = 2048;

    private int round;

    private List<Node> nodeList;
    private List<Block> blockList;

    public Vector2 Center { get; private set; }

    private enum GameState
    {
        GenerateLevel,
        SpawningBlocks,
        WaitingInputs,
        Moving,
        Win,
        Lose,
    }

    private GameState gameState;

    private void ChangeState(GameState newGameState)
    {
        gameState = newGameState;
        //print(gameState.ToString());

        switch (gameState)
        {
            case GameState.GenerateLevel:
                SetUpGame();
                break;
            case GameState.SpawningBlocks:
                SpawnBlocks(round++ == 0 ? 2 : 1);
                break;
            case GameState.WaitingInputs:
                break;
            case GameState.Moving:
                break;
            case GameState.Win:
                break;
            case GameState.Lose:
                break;
        }
    }

    private void Start()
    {
        ChangeState(GameState.GenerateLevel);
    }

    private void Update()
    {
        if (gameState != GameState.WaitingInputs) { return; }

        if (Input.GetKeyDown(KeyCode.LeftArrow)) { Shift(Vector2.left); }
        if (Input.GetKeyDown(KeyCode.RightArrow)) { Shift(Vector2.right); }
        if (Input.GetKeyDown(KeyCode.UpArrow)) { Shift(Vector2.up); }
        if (Input.GetKeyDown(KeyCode.DownArrow)) { Shift(Vector2.down); }
    }

    private void SetUpGame()
    {
        round = 0;
        nodeList = new();
        blockList = new();

        PlaceCamera();
        SpawnBoard();
        GenerateGrid();

        ChangeState(GameState.SpawningBlocks);
    }

    private void PlaceCamera()
    {
        Center = new Vector3(width / 2, height / 2, 0);
        Camera.main.transform.position = new Vector3(Center.x, Center.y, -10f);
    }

    private void SpawnBoard()
    {
        SpriteRenderer boardSpriteRenderer = Instantiate(boardPrefab, Center, Quaternion.identity);

        boardTransform = boardSpriteRenderer.transform;
        boardSpriteRenderer.size = new Vector2(width, height);
    }

    private void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float offSetFromCenter = 0.5f;
                Node node = Instantiate(nodePrefab, new Vector2(x + offSetFromCenter, y + offSetFromCenter), Quaternion.identity, boardTransform);
                nodeList.Add(node);
            }
        }
    }

    private void SpawnBlocks(int amount)
    {
        List<Node> freeNodes = nodeList.Where(node => node.OccupingBlock == null).OrderBy(node => Random.value).ToList();

        foreach (Node node in freeNodes.Take(amount))
        {
            SpawnBlock(node, Random.value > 0.8f ? 4 : 2);
        }

        if (freeNodes.Count == 1)
        {
            //Lost the game
            ChangeState(GameState.Lose);
            return;
        }

        ChangeState(blockList.Any(block => block.Value == winCondition) ? GameState.Win : GameState.WaitingInputs);
    }

    private void SpawnBlock(Node node, int value)
    {
        Block block = Instantiate(blockPrefab, node.Position, Quaternion.identity);
        block.Init(GetBlockTypeByValue(value));
        block.SetBlock(node);
        blockList.Add(block);
    }

    private void Shift(Vector2 direction)
    {
        Console.Clear();
        ChangeState(GameState.Moving);

        List<Block> orderedBlockList = blockList.OrderBy(block => block.Position.x).ThenBy(block => block.Position.y).ToList();

        if (direction == Vector2.right || direction == Vector2.up)
        {
            orderedBlockList.Reverse();
        }

        foreach (Block block in orderedBlockList)
        {
            Node next = block.OccupiedNode;
            do
            {
                block.SetBlock(next);
                Node possibleNode = GetNodeAtPosition(next.Position + direction);
                if (possibleNode != null)
                {
                    //A node is present
                    if (possibleNode.OccupingBlock != null
                        && possibleNode.OccupingBlock.CanMerge(block.Value))
                    {
                        //two blocks can marge

                        print("marge");
                        block.MergeBlock(possibleNode.OccupingBlock);

                    }
                    else if (possibleNode.OccupingBlock == null)
                    {
                        //move to the next node
                        print(".........next");
                        next = possibleNode;
                    }
                    print("+++++++++++++++++++");
                }
            } while (next != block.OccupiedNode);
        }

        Sequence sequence = DOTween.Sequence();
        float blockTravelTime = 0.2f;

        foreach (Block block in orderedBlockList)
        {
            Vector2 movePoint = block.BlockToMergeWith != null ? block.BlockToMergeWith.OccupiedNode.Position : block.OccupiedNode.Position;
            sequence.Insert(0, block.transform.DOMove(movePoint, blockTravelTime).SetEase(Ease.InQuad));
        }
        sequence.OnComplete(() =>
        {
            List<Block> margeBlockList = orderedBlockList.Where(block => block.BlockToMergeWith != null).ToList();
            foreach (Block block in margeBlockList)
            {
                MergeBlocks(block.BlockToMergeWith, block);
            }

            ChangeState(GameState.SpawningBlocks);
        });
    }


    private void MergeBlocks(Block baseBlock, Block mergingBlock)
    {
        int newValue = baseBlock.Value * 2;
        SpawnBlock(baseBlock.OccupiedNode, newValue);

        RemoveBlock(baseBlock);
        RemoveBlock(mergingBlock);
    }

    private void RemoveBlock(Block block)
    {
        blockList.Remove(block);
        Destroy(block.gameObject);
    }

    private Node GetNodeAtPosition(Vector2 position) => nodeList.FirstOrDefault(node => node.Position == position);

    private BlockType GetBlockTypeByValue(int value) => blockTypeList.First(type => type.Value == value);


}