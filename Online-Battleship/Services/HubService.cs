using Microsoft.AspNetCore.SignalR.Client;

namespace Online_Battleship.Services
{
    public class HubService
    {
        private HubConnection _connection;
        public static string HubUrl = $"{AppConfig.ServerUrl}/gamehub";
        public bool IsConnected => _connection?.State == HubConnectionState.Connected;

        // events
        public event Action<string, string, string, string, string> OnMatchFound;
        public event Action OnWaitingForOpponent;
        public event Action OnOpponentReady;
        public event Action<int, int> OnOpponentShot;
        public event Action<int, int, string, string> OnShotResult;
        public event Action<string, string> OnReceiveMessage;
        public event Action<int> OnGameEnded;
        public event Action<int, string> OnChallengeReceived;
        public event Action OnChallengeRejected;
        public event Action OnPlayerNotAvailable;
        public event Action<int, bool> OnPlayerStatusChanged;
        public event Action<bool> OnBothPlayersReady;

        public async Task Connect(int userId)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(HubUrl)
                .WithAutomaticReconnect()
                .Build();

            RegisterHandlers();

            await _connection.StartAsync();
            await _connection.InvokeAsync("RegisterUser", userId);
        }

        private void RegisterHandlers()
        {
            _connection.On<string, string, string, string, string>("MatchFound", (gameId, p1, p2, id1, id2) =>
            {
                OnMatchFound?.Invoke(gameId, p1, p2, id1, id2);
            });

            _connection.On("WaitingForOpponent", () =>
                OnWaitingForOpponent?.Invoke());

            _connection.On("OpponentReady", () =>
                OnOpponentReady?.Invoke());

            _connection.On<int, int>("OpponentShot", (row, col) =>
                OnOpponentShot?.Invoke(row, col));

            _connection.On<int, int, string, string>("ShotResult", (row, col, result, sunkShipInfo) =>
                OnShotResult?.Invoke(row, col, result, sunkShipInfo));

            _connection.On<string, string>("ReceiveMessage", (username, message) =>
                OnReceiveMessage?.Invoke(username, message));

            _connection.On<int>("GameEnded", (winnerId) =>
                OnGameEnded?.Invoke(winnerId));

            _connection.On<int, string>("ChallengeReceived", (challengerId, username) =>
                OnChallengeReceived?.Invoke(challengerId, username));

            _connection.On("ChallengeRejected", () =>
                OnChallengeRejected?.Invoke());

            _connection.On("PlayerNotAvailable", () =>
                OnPlayerNotAvailable?.Invoke());

            _connection.On<bool>("BothPlayersReady", (isFirst) =>
                OnBothPlayersReady?.Invoke(isFirst));
        }

        public async Task JoinMatchmaking(int userId) =>
            await _connection.InvokeAsync("JoinMatchmaking", userId);

        public async Task LeaveMatchmaking() =>
            await _connection.InvokeAsync("LeaveMatchmaking");

        public async Task ShipsReady(string gameId) =>
            await _connection.InvokeAsync("ShipsReady", gameId);

        public async Task Shoot(string gameId, int row, int col) =>
            await _connection.InvokeAsync("Shoot", gameId, row, col);

        public async Task ShotResult(string gameId, int row, int col, string result, string sunkShipInfo = "") =>
            await _connection.InvokeAsync("ShotResult", gameId, row, col, result, sunkShipInfo);
        public async Task SendMessage(string gameId, string message) =>
            await _connection.InvokeAsync("SendMessage", gameId, message);

        public async Task GameOver(string gameId, int winnerId, int loserId) =>
            await _connection.InvokeAsync("GameOver", gameId, winnerId, loserId);

        public async Task ChallengePlayer(int targetUserId) =>
            await _connection.InvokeAsync("ChallengePlayer", targetUserId);

        public async Task AcceptChallenge(int challengerId) =>
            await _connection.InvokeAsync("AcceptChallenge", challengerId);

        public async Task RejectChallenge(int challengerId) =>
            await _connection.InvokeAsync("RejectChallenge", challengerId);

        public async Task Disconnect()
        {
            if (_connection != null)
                await _connection.StopAsync();
        }
    }
}