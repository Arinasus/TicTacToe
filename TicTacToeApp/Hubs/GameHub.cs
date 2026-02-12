using Microsoft.AspNetCore.SignalR;

namespace TicTacToeApp.Hubs
{
    public class GameHub : Hub
    {
        public async Task SendMove(string gameId, string player, int x, int y) { 
            await Clients.Group(gameId).SendAsync("ReceiveMove", player, x, y); 
        }

        public async Task JoinGame(string gameId, string player) { 
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId); 
            await Clients.Group(gameId).SendAsync("PlayerJoined", player); 
        }
    }
}
