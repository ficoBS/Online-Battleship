namespace OnlineBattleship.Server.DTOs
{
    public class RegisterDTO
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponseDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
    }

    public class PlayerDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public bool IsOnline { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Points { get; set; }
    }

    public class MatchHistoryDTO
    {
        public int MatchId { get; set; }
        public string Player1Username { get; set; }
        public string Player2Username { get; set; }
        public string? WinnerUsername { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
    }
}