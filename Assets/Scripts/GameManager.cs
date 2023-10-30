using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform boardPrefab;
    [SerializeField] private Transform nodePrefab;
    [SerializeField] private Transform blockPrefab;
    [SerializeField] private List< BlockType> blockTypeList;

    private const int width = 4;
    private const int height = 4;
    private const int initialBlocksAmount = 2;
    private int round;

    private List<Node> nodeList = new();
    private List<Block> blockList = new();

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

        switch (gameState)
        {
            case GameState.GenerateLevel:
                SetUpGame();
                break;
            case GameState.SpawningBlocks:
                SpawnBlocks(round++ == 0? 2:1);
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

    public Vector3 Center { get; private set; }

    private void Start()
    {
        ChangeState(GameState.GenerateLevel);
    }

    private void Update()
    {
        
    }

    private void SetUpGame()
    {
        round = 0;
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
        Transform boardTransform = Instantiate(boardPrefab, Center, Quaternion.identity);
        if (boardTransform.TryGetComponent(out SpriteRenderer boardSpriteRenderer))
        {
            boardSpriteRenderer.size = new Vector2(width, height);
        }
    }

    private void GenerateGrid()
    {
        nodeList.Clear();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float offSetFromCenter = 0.5f;
                Transform nodeTransform = Instantiate(nodePrefab, new Vector3(x + offSetFromCenter, y + offSetFromCenter, 0), Quaternion.identity);
                if (nodeTransform.TryGetComponent(out Node node))
                {
                    nodeList.Add(node);
                }
            }
        }
    }

    private void SpawnBlocks(int amount)
    {
        blockList.Clear();

        List<Node> freeNodes = nodeList.Where(x => x.OccupiedBlock == null).OrderBy(y => Random.value).ToList();

        foreach (Node node in freeNodes.Take(amount))
        {
            Transform blockTransform = Instantiate(blockPrefab, node.Position, Quaternion.identity);
            if (blockTransform.TryGetComponent(out Block block))
            {
                block.Init(GetBlockTypeByValue(Random.value > 0.8f ? 4 : 2));
                //blockList.Add(block);
            }
        }

        if (freeNodes.Count == 1)
        {
            //Lost the game
            return;
        }
    }


    private BlockType GetBlockTypeByValue(int value) => blockTypeList.First(x => x.Value == value);


}
