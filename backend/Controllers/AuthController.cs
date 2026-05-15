using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using TourPlanner.backend.DTOs;
using TourPlanner.backend.Entities;
using TourPlanner.backend.Repositories;

namespace TourPlanner.backend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    // În C# activăm CORS în Program.cs, dar putem pune și aici [EnableCors]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthController(IUserRepository userRepository, IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
        {
            var existingUser = await _userRepository.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "Email already in use" });
            }

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email
            };

            user.Password = _passwordHasher.HashPassword(user, dto.Password);

            await _userRepository.AddAsync(user);
            await _userRepository.SaveAsync(); 

            return Ok(new { message = "User registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            var user = await _userRepository.FindByEmailAsync(dto.Email);
            
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, dto.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            return Ok(new
            {
                token = "dummy-jwt-token",
                userId = user.Id,
                username = user.Username
            });
        }
    }

    // Un DTO mic pentru Login, deoarece C# preferă obiecte tipizate în loc de Map<string, string>
    public class LoginRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}