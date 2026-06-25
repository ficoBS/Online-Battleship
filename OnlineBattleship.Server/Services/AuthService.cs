using Microsoft.EntityFrameworkCore;
using OnlineBattleship.Server.Data;
using OnlineBattleship.Server.DTOs;
using OnlineBattleship.Server.Models;
using System.Security.Cryptography;
using System.Text;

namespace OnlineBattleship.Server.Services
{
    public class AuthService
    {
        private readonly AppDbContext _db;

        public AuthService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<bool> Register(RegisterDTO dto)
        {
            bool exists = await _db.Users.AnyAsync(u => u.Username == dto.Username || u.Email == dto.Email);
            if (exists) return false;

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password),
                Wins = 0,
                Losses = 0,
                Points = 0,
                IsOnline = false,
                CreatedAt = DateTime.Now,
                LastSeen = DateTime.Now
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<(User? user, string error)> Login(LoginDTO dto)
        {
            string hash = HashPassword(dto.Password);
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email && u.PasswordHash == hash);

            if (user == null) return (null, "Invalid credentials");
            if (user.IsOnline) return (null, "Account is already logged in");

            user.IsOnline = true;
            user.LastSeen = DateTime.Now;
            await _db.SaveChangesAsync();

            return (user, "");
        }

        public async Task Logout(int userId)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null) return;

            user.IsOnline = false;
            user.LastSeen = DateTime.Now;
            await _db.SaveChangesAsync();
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes);
        }
    }
}