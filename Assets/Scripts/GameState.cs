public partial class GameManager
{
    private enum GameState
    {
        GenerateBoard,
        GenetateGrid,
        SpawningBlocks,
        WaitingInput,
        Moving,
        Win,
        Lose,
    }


}


//TODO: continious input
//TODO: starting can have (2,2)/(2,4)/(4,4) blocks
//TODO: if no move and/or no merge == no new spawn
//TODO: add game loop
//TODO: add sound and music
//TODO: game Over login bug fix
//TODO: support generic 