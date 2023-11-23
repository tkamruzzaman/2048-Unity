using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event EventHandler OnGameReload;

    public event EventHandler<bool> OnGameOver;

    public event EventHandler<OnBlocksMergedEventArgs> OnBlocksMerged;
    public class OnBlocksMergedEventArgs : EventArgs
    {
        public Block BlockA;
        public Block BlockB;
        public int newValue;
    }

    public event EventHandler OnAnyBlockMerged;
    public event EventHandler OnValidMove;
    public event EventHandler OnInvalidMove;

    [SerializeField] private Board boardPrefab;
    [SerializeField] private Node nodePrefab;
    [SerializeField] private Block blockPrefab;
    [Space]
    [SerializeField] private Transform boardParentTransform;
    [Space]
    [SerializeField] private List<BlockType> blockTypeList;
    
    private List<Node> nodeList;
    private List<Block> blockList;

    private Board board;

    private GameState gameState;

    private int winCondition;
    private int gameRound;

    private bool isMerging;
    private bool isMoving;
    private bool isValidInput;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one GameManager" + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        winCondition = blockTypeList[^1].Value; //last value of the list 
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
                OnGameOver?.Invoke(this, true);
                break;
            case GameState.Lose:
                OnGameOver?.Invoke(this, false);
                break;
        }
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
        board = Instantiate(boardPrefab, boardParentTransform);
        board.Init();
    }

    private void GenerateGrid()
    {
        for (int x = 0; x < board.Width; x++)
        {
            for (int y = 0; y < board.Height; y++)
            {
                float offSetFromCenter = 0.5f;
                Node node = Instantiate(nodePrefab,
                    new Vector2(x + offSetFromCenter, y + offSetFromCenter),
                    Quaternion.identity, boardParentTransform);

                GridPosition gridPosition = new(x, y);
                board.AddNodeToGridPosition(gridPosition, node);
                node.InitNode(gridPosition);
                nodeList.Add(node);
            }
        }

        ChangeState(GameState.SpawningBlocks);
    }

    private void SpawnBlocks(int amount)
    {
        if (isValidInput)
        {
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
            if (!IsAnyMoveAvailable())
            {
                //Lost the game
                ChangeState(GameState.Lose);
                return;
            }
        }

        gameRound++;

        ChangeState(blockList.Any(block => block.Value == winCondition) ? GameState.Win : GameState.WaitingInput);
    }

    private bool IsAnyMoveAvailable()
    {
        List<GridPosition> validGridPositionList = new();

        int maxTestDistance = 1;
        foreach (Block block in blockList)
        {
            GridPosition blockGridPosition = block.OccupiedNode.GridPosition;

            for (int x = -maxTestDistance; x <= maxTestDistance; x++)
            {
                for (int y = -maxTestDistance; y <= maxTestDistance; y++)
                {
                    GridPosition offsetGridPosition = new(x, y);
                    GridPosition testGridPosition = blockGridPosition + offsetGridPosition;

                    if (Mathf.Abs(x) == Mathf.Abs(y))
                    {
                        //diagonal value
                        continue;
                    }

                    if (!board.IsValidGridPosition(testGridPosition))
                    {
                        //invalid GridPosition
                        continue;
                    }

                    Block targetBlock = board.GetNodeAtGridPosition(testGridPosition).OccupingBlock;
                    if (targetBlock == null)
                    {
                        //No block on the node
                        continue;
                    }

                    if (targetBlock == block)
                    {
                        //same block
                        continue;
                    }

                    if (targetBlock.Value != block.Value)
                    {
                        //Can not be matched
                        continue;
                    }

                    validGridPositionList.Add(testGridPosition);
                }
            }
        }
        return validGridPositionList.Count > 0;
    }

    private void SpawnBlock(Node node, int value)
    {
        Block block = Instantiate(blockPrefab, node.Position, Quaternion.identity, boardParentTransform);
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

        isMerging = isMoving = isValidInput = false;

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

                    isValidInput = !(isMerging || isMoving); //N-OR
                }
            } while (next != block.OccupiedNode);
        }

        if (isValidInput) { OnInvalidMove?.Invoke(this, EventArgs.Empty); }
        else { OnValidMove?.Invoke(this, EventArgs.Empty); }

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
            if (margeBlockList.Any()) 
            {
                //at least one block merge
                OnAnyBlockMerged?.Invoke(this, EventArgs.Empty);
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

        OnBlocksMerged?.Invoke(this, new OnBlocksMergedEventArgs
        {
            BlockA = baseBlock,
            BlockB = mergingBlock,
            newValue = newValue
        });
    }

    private void RemoveBlock(Block block)
    {
        blockList.Remove(block);
        Destroy(block.gameObject);
    }

    private int GetBlockAmount() => gameRound == 0 ? 2 : 1;

    private int GetBlockValue() => Random.value > (gameRound == 0 ? 0.6f : 0.8f) ? 4 : 2;

    private Node GetNodeAtPosition(Vector2 position) => nodeList.FirstOrDefault(node => node.Position == position);

    public BlockType GetBlockTypeByValue(int value) => blockTypeList.First(type => type.Value == value);

    [ContextMenu("ReloadLevel")]
    public void ReloadLevel()
    {
        for (int i = 0; i < nodeList.Count; i++)
        {
            Destroy(nodeList[i].gameObject);
        }

        for (int i = 0; i < blockList.Count; i++)
        {
            Destroy(blockList[i].gameObject);
        }

        blockList.Clear();
        nodeList.Clear();

        isMerging = isMoving = isValidInput = false;

        board = null;
        gameRound = 0;

        ChangeState(GameState.GenerateBoard);

        OnGameReload?.Invoke(this, EventArgs.Empty);
    }
}


//TODO: continious input => done
//TODO: starting can have (2,2)/(2,4)/(4,4) blocks => done
//TODO: if no move and/or no merge == no new spawn => done
//TODO: add game loop => done
//TODO: add sound and music => done
//TODO: game Over logic bug fix => done
//TODO: support generic 