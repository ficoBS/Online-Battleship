using Online_Battleship.Models;

namespace Online_Battleship.Services
{
    public static class SessionService
    {
        public static int UserId { get; set; }
        public static string Username { get; set; }
        public static string CurrentGameId { get; set; }
        public static int OpponentId { get; set; }
        public static string OpponentUsername { get; set; }

        public static ApiService Api { get; private set; } = new ApiService();
        public static HubService Hub { get; private set; } = new HubService();

        public static Board PlayerBoard { get; set; }
        public static void Clear()
        {
            UserId = 0;
            Username = "";
            CurrentGameId = "";
            OpponentId = 0;
            OpponentUsername = "";
        }
    }
}