namespace OnlineBattleship.Server.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Points { get; set; }
        public bool IsOnline { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastSeen { get; set; }
    }
}