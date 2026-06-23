using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineBattleship.Server.Data;
using OnlineBattleship.Server.DTOs;

namespace OnlineBattleship.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly AppDbContext _db;

        public PlayersController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetPlayers()
        {
            var players = await _db.Users
                .Select(u => new PlayerDTO
                {
                    Id = u.Id,
                    Username = u.Username,
                    IsOnline = u.IsOnline,
                    Wins = u.Wins,
                    Losses = u.Losses,
                    Points = u.Points
                })
                .OrderByDescending(u => u.Points)
                .ToListAsync();

            return Ok(players);
        }

        [HttpGet("online")]
        public async Task<IActionResult> GetOnlinePlayers()
        {
            var players = await _db.Users
                .Where(u => u.IsOnline)
                .Select(u => new PlayerDTO
                {
                    Id = u.Id,
                    Username = u.Username,
                    IsOnline = u.IsOnline,
                    Wins = u.Wins,
                    Losses = u.Losses,
                    Points = u.Points
                })
                .ToListAsync();

            return Ok(players);
        }

        [HttpGet("leaderboard")]
        public async Task<IActionResult> GetLeaderboard()
        {
            var players = await _db.Users
                .OrderByDescending(u => u.Wins)
                .Take(10)
                .Select(u => new PlayerDTO
                {
                    Id = u.Id,
                    Username = u.Username,
                    Wins = u.Wins,
                    Losses = u.Losses,
                    Points = u.Points
                })
                .ToListAsync();

            return Ok(players);
        }
    }
}