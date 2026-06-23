using Online_Battleship.Models;
using System.Net.Http.Json;

namespace Online_Battleship.Services
{
    public class ApiService
    {
        private readonly HttpClient _client;
        public static string BaseUrl = "http://localhost:5000";

        public ApiService()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(BaseUrl);
        }

        public async Task<(bool success, string message, int userId, string username)> Login(string email, string password)
        {
            try
            {
                var response = await _client.PostAsJsonAsync("/api/auth/login", new
                {
                    email,
                    password
                });

                if (!response.IsSuccessStatusCode)
                    return (false, "Invalid credentials", 0, "");

                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                return (true, "", result.UserId, result.Username);
            }
            catch
            {
                return (false, "Cannot connect to server", 0, "");
            }
        }

        public async Task<(bool success, string message)> Register(string username, string email, string password)
        {
            try
            {
                var response = await _client.PostAsJsonAsync("/api/auth/register", new
                {
                    username,
                    email,
                    password
                });

                if (!response.IsSuccessStatusCode)
                    return (false, "Username or email already exists");

                return (true, "");
            }
            catch
            {
                return (false, "Cannot connect to server");
            }
        }

        public async Task Logout(int userId)
        {
            try
            {
                await _client.PostAsync($"/api/auth/logout/{userId}", null);
            }
            catch { }
        }

        public async Task<List<PlayerDto>> GetPlayers()
        {
            try
            {
                return await _client.GetFromJsonAsync<List<PlayerDto>>("/api/players") ?? new List<PlayerDto>();
            }
            catch
            {
                return new List<PlayerDto>();
            }
        }

        public async Task<List<PlayerDto>> GetLeaderboard()
        {
            try
            {
                return await _client.GetFromJsonAsync<List<PlayerDto>>("/api/players/leaderboard") ?? new List<PlayerDto>();
            }
            catch
            {
                return new List<PlayerDto>();
            }
        }

        public async Task<List<MatchHistoryDto>> GetMatchHistory(int userId)
        {
            try
            {
                return await _client.GetFromJsonAsync<List<MatchHistoryDto>>($"/api/matchhistory/{userId}") ?? new List<MatchHistoryDto>();
            }
            catch
            {
                return new List<MatchHistoryDto>();
            }
        }

        private class LoginResponse
        {
            public int UserId { get; set; }
            public string Username { get; set; }
            public string Token { get; set; }
        }
    }

    public class PlayerDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public bool IsOnline { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Points { get; set; }
    }

    public class MatchHistoryDto
    {
        public int MatchId { get; set; }
        public string Player1Username { get; set; }
        public string Player2Username { get; set; }
        public string WinnerUsername { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
    }
}