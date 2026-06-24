using Microsoft.AspNetCore.SignalR;
using OnlineBattleship.Server.Data;
using OnlineBattleship.Server.DTOs;
using Microsoft.EntityFrameworkCore;

namespace OnlineBattleship.Server.Hubs
{
    public class GameHub : Hub
    {
        private readonly AppDbContext _db;
        private static Dictionary<string, int> _connectedUsers = new();
        private static Dictionary<string, List<string>> _gameGroups = new();
        private static Queue<(string connectionId, int userId)> _matchmakingQueue = new();

        public GameHub(AppDbContext db)
        {
            _db = db;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (_connectedUsers.TryGetValue(Context.ConnectionId, out int userId))
            {
                var user = await _db.Users.FindAsync(userId);
                if (user != null)
                {
                    user.IsOnline = false;
                    user.LastSeen = DateTime.Now;
                    await _db.SaveChangesAsync();
                }
                _connectedUsers.Remove(Context.ConnectionId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task RegisterUser(int userId)
        {
            _connectedUsers[Context.ConnectionId] = userId;

            var user = await _db.Users.FindAsync(userId);
            if (user != null)
            {
                user.IsOnline = true;
                await _db.SaveChangesAsync();
            }

            await Clients.All.SendAsync("PlayerStatusChanged", new PlayerDTO
            {
                Id = userId,
                IsOnline = true
            });
        }

        public async Task JoinMatchmaking(int userId)
        {
            _matchmakingQueue.Enqueue((Context.ConnectionId, userId));

            if (_matchmakingQueue.Count >= 2)
            {
                var player1 = _matchmakingQueue.Dequeue();
                var player2 = _matchmakingQueue.Dequeue();

                string gameId = Guid.NewGuid().ToString();

                await Groups.AddToGroupAsync(player1.connectionId, gameId);
                await Groups.AddToGroupAsync(player2.connectionId, gameId);

                _gameGroups[gameId] = new List<string> { player1.connectionId, player2.connectionId };

                var user1 = await _db.Users.FindAsync(player1.userId);
                var user2 = await _db.Users.FindAsync(player2.userId);

                await Clients.Group(gameId).SendAsync("MatchFound", gameId, user1?.Username ?? "", user2?.Username ?? "");
            }
            else
            {
                await Clients.Caller.SendAsync("WaitingForOpponent");
            }
        }

        public async Task LeaveMatchmaking()
        {
            var updated = _matchmakingQueue
                .Where(q => q.connectionId != Context.ConnectionId)
                .ToList();
            _matchmakingQueue.Clear();
            foreach (var item in updated)
                _matchmakingQueue.Enqueue(item);

            await Clients.Caller.SendAsync("MatchmakingCancelled");
        }

        private static Dictionary<string, int> _readyPlayers = new();

        public async Task ShipsReady(string gameId)
        {
            if (!_readyPlayers.ContainsKey(gameId))
                _readyPlayers[gameId] = 0;

            _readyPlayers[gameId]++;

            if (_readyPlayers[gameId] >= 2)
            {
                _readyPlayers.Remove(gameId);
                await Clients.Group(gameId).SendAsync("BothPlayersReady");
            }
            else
            {
                await Clients.OthersInGroup(gameId).SendAsync("OpponentReady");
            }
        }

        public async Task Shoot(string gameId, int row, int col)
        {
            await Clients.OthersInGroup(gameId).SendAsync("OpponentShot", row, col);
        }

        public async Task ShotResult(string gameId, int row, int col, string result)
        {
            await Clients.OthersInGroup(gameId).SendAsync("ShotResult", row, col, result);
        }

        public async Task SendMessage(string gameId, string message)
        {
            if (!_connectedUsers.TryGetValue(Context.ConnectionId, out int userId)) return;

            var user = await _db.Users.FindAsync(userId);
            await Clients.Group(gameId).SendAsync("ReceiveMessage", user?.Username, message);
        }

        public async Task GameOver(string gameId, int winnerId, int loserId)
        {
            var winner = await _db.Users.FindAsync(winnerId);
            var loser = await _db.Users.FindAsync(loserId);

            if (winner != null)
            {
                winner.Wins++;
                winner.Points += 10;
            }
            if (loser != null)
            {
                loser.Losses++;
            }

            var match = new Models.Match
            {
                Player1Id = winnerId,
                Player2Id = loserId,
                WinnerId = winnerId,
                StartedAt = DateTime.Now,
                EndedAt = DateTime.Now
            };

            _db.Matches.Add(match);
            await _db.SaveChangesAsync();

            await Clients.Group(gameId).SendAsync("GameEnded", winnerId);
        }

        public async Task ChallengePlayer(int targetUserId)
        {
            var targetConnection = _connectedUsers
                .FirstOrDefault(x => x.Value == targetUserId).Key;

            if (targetConnection == null)
            {
                await Clients.Caller.SendAsync("PlayerNotAvailable");
                return;
            }

            if (!_connectedUsers.TryGetValue(Context.ConnectionId, out int challengerId)) return;
            var challenger = await _db.Users.FindAsync(challengerId);

            await Clients.Client(targetConnection).SendAsync("ChallengeReceived", challengerId, challenger?.Username);
        }

        public async Task AcceptChallenge(int challengerId)
        {
            var challengerConnection = _connectedUsers
                .FirstOrDefault(x => x.Value == challengerId).Key;

            if (challengerConnection == null) return;

            var updated = _matchmakingQueue
                .Where(q => q.connectionId != Context.ConnectionId && q.connectionId != challengerConnection)
                .ToList();
            _matchmakingQueue.Clear();
            foreach (var item in updated)
                _matchmakingQueue.Enqueue(item);

            string gameId = Guid.NewGuid().ToString();

            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            await Groups.AddToGroupAsync(challengerConnection, gameId);

            _gameGroups[gameId] = new List<string> { Context.ConnectionId, challengerConnection };

            var user1 = await _db.Users.FindAsync(challengerId);
            if (!_connectedUsers.TryGetValue(Context.ConnectionId, out int accepterId)) return;
            var user2 = await _db.Users.FindAsync(accepterId);

            await Clients.Group(gameId).SendAsync("MatchFound", gameId, user1?.Username ?? "", user2?.Username ?? "");
        }
        public async Task RejectChallenge(int challengerId)
        {
            var challengerConnection = _connectedUsers
                .FirstOrDefault(x => x.Value == challengerId).Key;

            if (challengerConnection == null) return;

            await Clients.Client(challengerConnection).SendAsync("ChallengeRejected");
        }
    }
}