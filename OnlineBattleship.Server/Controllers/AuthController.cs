using Microsoft.AspNetCore.Mvc;
using OnlineBattleship.Server.DTOs;
using OnlineBattleship.Server.Services;

namespace OnlineBattleship.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            bool success = await _authService.Register(dto);
            if (!success) return BadRequest(new { message = "Username or email already exists" });
            return Ok(new { success = true });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var user = await _authService.Login(dto);
            if (user == null) return Unauthorized(new { message = "Invalid credentials" });

            return Ok(new LoginResponseDTO
            {
                UserId = user.Id,
                Username = user.Username,
                Token = user.Id.ToString()
            });
        }

        [HttpPost("logout/{userId}")]
        public async Task<IActionResult> Logout(int userId)
        {
            await _authService.Logout(userId);
            return Ok(new { success = true });
        }
    }
}