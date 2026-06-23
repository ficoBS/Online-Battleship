using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineBattleship.Server.Data;
using OnlineBattleship.Server.DTOs;

namespace OnlineBattleship.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatchHistoryController : ControllerBase
    {
        private readonly AppDbContext _db;

        public MatchHistoryController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetHistory(int userId)
        {
            var matches = await _db.Matches
                .Include(m => m.Player1)
                .Include(m => m.Player2)
                .Include(m => m.Winner)
                .Where(m => m.Player1Id == userId || m.Player2Id == userId)
                .OrderByDescending(m => m.StartedAt)
                .Select(m => new MatchHistoryDTO
                {
                    MatchId = m.Id,
                    Player1Username = m.Player1.Username,
                    Player2Username = m.Player2.Username,
                    WinnerUsername = m.Winner != null ? m.Winner.Username : null,
                    StartedAt = m.StartedAt,
                    EndedAt = m.EndedAt
                })
                .ToListAsync();

            return Ok(matches);
        }
    }
}