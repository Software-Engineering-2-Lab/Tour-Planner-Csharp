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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser([FromRoute] long id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(new UserProfileResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                TotalTours = user.Tours?.Count ?? 0
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser([FromRoute] long id, [FromBody] UserUpdateDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            if (!string.IsNullOrWhiteSpace(dto.Username))
                user.Username = dto.Username;

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var existing = await _userRepository.FindByEmailAsync(dto.Email);
                if (existing != null && existing.Id != id)
                    return BadRequest(new { message = "Email already in use" });
                user.Email = dto.Email;
            }

            if (!string.IsNullOrWhiteSpace(dto.Password))
                user.Password = _passwordHasher.HashPassword(user, dto.Password);

            _userRepository.Update(user);
            await _userRepository.SaveAsync();

            return Ok(new UserProfileResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                TotalTours = user.Tours?.Count ?? 0
            });
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

            var claims = new System.Security.Claims.Claim[]
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString()),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Username)
            };

            var key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("ExtraMegaSuperSecureSecurityTokenKey"));
            var creds = new Microsoft.IdentityModel.Tokens.SigningCredentials(key, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(claims),
                Expires = System.DateTime.UtcNow.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var realToken = tokenHandler.WriteToken(securityToken);

            return Ok(new
            {
                token = realToken,
                userId = user.Id,
                username = user.Username,
                email = user.Email 
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