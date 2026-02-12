using Microsoft.AspNetCore.SignalR;
using TicTacToe.Shared;

namespace TicTacToe.Server.Hubs
{
    public class GameState
    {
        public string[,] Board { get; set; } = new string[3, 3];
        public string CurrentTurn { get; set; } = "X";
        public bool GameOver { get; set; } = false;
        public Dictionary<string, string> Players { get; set; } = new();
        public Dictionary<string, string> PlayerNames { get; set; } = new();
        public string CreatedBy { get; set; }
    }

    public class GameHub : Hub
    {
        private static Dictionary<string, GameState> games = new();

        public async Task<string> CreateGame(string playerName)
        {
            string gameId = Guid.NewGuid().ToString(); 
            var game = new GameState { CreatedBy = playerName }; 
            games[gameId] = game; 
            game.Players[Context.ConnectionId] = "X"; 
            game.PlayerNames[Context.ConnectionId] = playerName; 
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId); 
            await Clients.Caller.SendAsync("AssignSymbol", "X"); 
            await Clients.Group(gameId).SendAsync("PlayerJoined", "X", game.Players.Count); 
            return gameId;
        }

        public async Task<List<GameInfo>> ListGames()
        {
            return games.Select(g => new GameInfo
            {
                GameId = g.Key,
                CreatedBy = g.Value.CreatedBy,
                PlayerCount = g.Value.Players.Count,
                GameOver = g.Value.GameOver,
                Players = g.Value.PlayerNames.Values.ToList()
            }).ToList();
        }

        public async Task JoinGame(string gameId, string playerName) { 
            if (string.IsNullOrEmpty(gameId)) 
                return; 
            if (!games.ContainsKey(gameId))
                return; var game = games[gameId]; 
            if (game.Players.Count >= 2) 
                return; 
            string symbol; 
            if (!game.Players.Values.Contains("X")) symbol = "X"; 
            else if (!game.Players.Values.Contains("O")) symbol = "O"; 
            else symbol = "?"; game.Players[Context.ConnectionId] = symbol; 
            game.PlayerNames[Context.ConnectionId] = playerName; 
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId); 
            await Clients.Caller.SendAsync("AssignSymbol", symbol); 
            await Clients.Group(gameId).SendAsync("PlayerJoined", symbol, game.Players.Count); 
        }

        public async Task SendMove(string gameId, string player, int x, int y)
        {
            if (string.IsNullOrEmpty(gameId) || !games.ContainsKey(gameId)) return;
            var game = games[gameId];

            if (game.GameOver) return;
            if (player != game.CurrentTurn) return;
            if (!string.IsNullOrEmpty(game.Board[x, y])) return;

            game.Board[x, y] = player;
            await Clients.Group(gameId).SendAsync("ReceiveMove", player, x, y);

            if (CheckWin(game.Board, player))
            {
                game.GameOver = true;
                await Clients.Group(gameId).SendAsync("GameOver", player);
                return;
            }
            else if (CheckDraw(game.Board))
            {
                game.GameOver = true;
                await Clients.Group(gameId).SendAsync("GameOver", "Draw");
                return;
            }

            game.CurrentTurn = game.CurrentTurn == "X" ? "O" : "X";
            await Clients.Group(gameId).SendAsync("TurnChanged", game.CurrentTurn);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            foreach (var kv in games.ToList())
            {
                if (kv.Value.Players.ContainsKey(Context.ConnectionId))
                {
                    kv.Value.Players.Remove(Context.ConnectionId);

                    // если оба вышли и игра завершена — удаляем комнату
                    if (kv.Value.Players.Count == 0 && kv.Value.GameOver)
                    {
                        games.Remove(kv.Key);
                    }
                }
            }
            await base.OnDisconnectedAsync(exception);
        }

        private bool CheckWin(string[,] board, string symbol)
        {
            for (int i = 0; i < 3; i++)
            {
                if (board[i, 0] == symbol && board[i, 1] == symbol && board[i, 2] == symbol) return true;
                if (board[0, i] == symbol && board[1, i] == symbol && board[2, i] == symbol) return true;
            }
            if (board[0, 0] == symbol && board[1, 1] == symbol && board[2, 2] == symbol) return true;
            if (board[0, 2] == symbol && board[1, 1] == symbol && board[2, 0] == symbol) return true;
            return false;
        }

        private bool CheckDraw(string[,] board)
        {
            foreach (var cell in board)
            {
                if (string.IsNullOrEmpty(cell)) return false;
            }
            return true;
        }
    }
}
