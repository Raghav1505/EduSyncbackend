using System;
using System.Text;                      // <-- for Encoding.UTF8
using System.Security.Claims;           // <-- for Claim and ClaimTypes
using System.IdentityModel.Tokens.Jwt;  // <-- for JwtSecurityToken
using Microsoft.IdentityModel.Tokens;   // <-- for SymmetricSecurityKey, SigningCredentials
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using EduSync.Data;
using EduSync.DTOs;


namespace EduSync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        /// <summary>
        /// Authenticates a user and returns a JWT.
        /// </summary>
        /// <param name="dto">LoginDto containing email and password.</param>
        /// <returns>JWT token if successful, 401 otherwise.</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 1. Find the user by email
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return Unauthorized("Invalid credentials");

            // 2. Verify password (using SHA256 hash comparison)
            using var sha = System.Security.Cryptography.SHA256.Create();
            var computedHash = Convert.ToBase64String(sha.ComputeHash(
                Encoding.UTF8.GetBytes(dto.Password)));
            if (computedHash != user.PasswordHash)
                return Unauthorized("Invalid credentials");

            // 3. Create JWT claims
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,   user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(ClaimTypes.Role,               user.Role!)
            };

            // 4. Read JWT settings from configuration
            var jwtSection = _config.GetSection("Jwt");
            var keyBytes = Encoding.UTF8.GetBytes(jwtSection["Key"]!);
            var creds = new SigningCredentials(
                                new SymmetricSecurityKey(keyBytes),
                                SecurityAlgorithms.HmacSha256);

            // 5. Generate the token
            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                                       double.Parse(jwtSection["ExpiresInMinutes"]!)),
                signingCredentials: creds
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // 6. Return the token
            return Ok(new { token = tokenString });
        }
    }
}
