namespace TicTacToe.Shared
{
    public class GameInfo { 
        public string GameId { get; set; }
        public string CreatedBy { get; set; }
        public int PlayerCount { get; set; } 
        public bool GameOver { get; set; }
        public List<string> Players { get; set; } = new();
    }
}
