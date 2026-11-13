using HemCompetition.API.Data;
using HemCompetition.API.DTOs;
using HemCompetition.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HemCompetition.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminAuthController : ControllerBase
    {
        private readonly AuthDbContext _context;
        private readonly IConfiguration _configuration;

        public AdminAuthController(
            AuthDbContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // POST: /api/AdminAuth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] AdminLoginDto model)
        {
            if (model == null ||
                string.IsNullOrWhiteSpace(model.Username) ||
                string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest(new
                {
                    message = "Username and Password are required."
                });
            }

            var username = model.Username.Trim();

            var admin = await _context.AdminUsers
                .FirstOrDefaultAsync(x => x.Username == username);

            if (admin == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid admin username or password."
                });
            }

            if (!admin.IsActive)
            {
                return Unauthorized(new
                {
                    message = "This admin account is inactive."
                });
            }

            var passwordHasher = new PasswordHasher<AdminUser>();

            var result = passwordHasher.VerifyHashedPassword(
                admin,
                admin.PasswordHash,
                model.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                admin.AccessFailedCount++;
                admin.UpdatedAtUtc = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Unauthorized(new
                {
                    message = "Invalid admin username or password."
                });
            }

            admin.AccessFailedCount = 0;
            admin.LastLoginAtUtc = DateTime.UtcNow;
            admin.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var jwtKey = _configuration["Jwt:Key"];

            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                return StatusCode(500, new
                {
                    message = "Authentication configuration is invalid."
                });
            }

            if (!double.TryParse(
                    _configuration["Jwt:ExpireMinutes"],
                    out var expireMinutes))
            {
                expireMinutes = 60;
            }

            var key = Encoding.UTF8.GetBytes(jwtKey);

            // ==========================================
            // ADMIN CLAIMS
            // ==========================================

            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    admin.Id.ToString()),

                new Claim(
                    ClaimTypes.Name,
                    admin.Username),

                new Claim(
                    "AdminId",
                    admin.Id.ToString()),

                // HARD-CODED ADMIN ROLE
                new Claim(
                    "AdminRole",
                    "Admin"),

                // Also keep standard ASP.NET role
                new Claim(
                    ClaimTypes.Role,
                    "Admin")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),

                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),

                Issuer = _configuration["Jwt:Issuer"],

                Audience = _configuration["Jwt:Audience"],

                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                token = tokenString,

                message = "Admin login successful.",

                profile = new
                {
                    id = admin.Id,
                    username = admin.Username,
                    displayName = admin.DisplayName,
                    role = "Admin"
                }
            });
        }
    }
}