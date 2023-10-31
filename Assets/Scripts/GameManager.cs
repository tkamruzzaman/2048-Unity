using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class GameManager : MonoBehaviour
{
    [SerializeField] private Board boardPrefab;
    [SerializeField] private Node nodePrefab;
    [SerializeField] private Block blockPrefab;
    [Space]
    [SerializeField] private Transform boardTransform;
    [Space]
    [SerializeField] private List<BlockType> blockTypeList;

    private const int winCondition = 2048;

    private int gameRound;

    private List<Node> nodeList;
    private List<Block> blockList;

    private Board board;

    private GameState gameState;

    private bool isMerging;
    private bool isMoving;
    private bool isToSkipSpawning;

    private void ChangeState(GameState newGameState)
    {
        gameState = newGameState;

        switch (gameState)
        {
            case GameState.GenerateBoard:
                SetUpGame();
                break;
            case GameState.GenetateGrid:
                GenerateGrid();
                break;
            case GameState.SpawningBlocks:
                SpawnBlocks(GetBlockAmount());
                break;
            case GameState.WaitingInput:
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
        ChangeState(GameState.GenerateBoard);
    }

    private void Update()
    {
        if (gameState != GameState.WaitingInput) { return; }

        Shift(InputManager.Instance.GetMoveDirectionVector());
    }

    private void SetUpGame()
    {
        gameRound = 0;
        nodeList = new();
        blockList = new();

        SpawnBoard();

        ChangeState(GameState.GenetateGrid);
    }

    private void SpawnBoard()
    {
        board = Instantiate(boardPrefab);
        boardTransform = board.transform;
        board.Init();
    }

    private void GenerateGrid()
    {
        for (int x = 0; x < board.Width; x++)
        {
            for (int y = 0; y < board.Height; y++)
            {
                float offSetFromCenter = 0.5f;
                Node node = Instantiate(nodePrefab, new Vector2(x + offSetFromCenter, y + offSetFromCenter), Quaternion.identity, boardTransform);
                nodeList.Add(node);
            }
        }

        ChangeState(GameState.SpawningBlocks);
    }

    private void SpawnBlocks(int amount)
    {
        if (isToSkipSpawning)
        {
            //isToSkipSpawning = false;
            ChangeState(GameState.WaitingInput);
            return;
        }

        List<Node> freeNodes = nodeList.Where(node => node.OccupingBlock == null).OrderBy(node => Random.value).ToList();

        foreach (Node node in freeNodes.Take(amount))
        {
            SpawnBlock(node, GetBlockValue());
        }

        if (freeNodes.Count == 1)
        {
            bool anyDuplicate = nodeList.GroupBy(node => node.OccupingBlock.Value).Any(group => group.Count() > 1);

            //Lost the game
            ChangeState(GameState.Lose);
            return;

        }

        gameRound++;

        ChangeState(blockList.Any(block => block.Value == winCondition) ? GameState.Win : GameState.WaitingInput);
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
        if (direction == Vector2.zero) { return; }    //stop it from auto playing

        ChangeState(GameState.Moving);

        List<Block> orderedBlockList = blockList.OrderBy(block => block.Position.x).ThenBy(block => block.Position.y).ToList();

        if (direction == Vector2.right || direction == Vector2.up)
        {
            orderedBlockList.Reverse();
        }

        isMerging = isMoving = isToSkipSpawning = false;

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
                        isMerging = true;
                        block.MergeBlock(possibleNode.OccupingBlock);

                    }
                    else if (possibleNode.OccupingBlock == null)
                    {
                        //move to the next node
                        isMoving = true;
                        next = possibleNode;
                    }

                    isToSkipSpawning = !(isMerging || isMoving); //N-OR
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

    private int GetBlockAmount() => gameRound == 0 ? 2 : 1;

    private int GetBlockValue() => Random.value > (gameRound == 0 ? 0.6f : 0.8f) ? 4 : 2;

    private Node GetNodeAtPosition(Vector2 position) => nodeList.FirstOrDefault(node => node.Position == position);

    private BlockType GetBlockTypeByValue(int value) => blockTypeList.First(type => type.Value == value);


}


//TODO: continious input => done
//TODO: starting can have (2,2)/(2,4)/(4,4) blocks => done
//TODO: if no move and/or no merge == no new spawn => done
//TODO: add game loop
//TODO: add sound and music
//TODO: game Over login bug fix
//TODO: support generic 