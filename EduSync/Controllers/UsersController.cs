using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EduSync.Data;
using EduSync.DTOs;
using EduSync.Models;

namespace EduSync.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // everything here requires a valid JWT
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // Allow anyone to register (self-signup)
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> PostUser([FromBody] CreateUserDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // hash the password
            using var sha = System.Security.Cryptography.SHA256.Create();
            var hash = Convert.ToBase64String(sha.ComputeHash(
                System.Text.Encoding.UTF8.GetBytes(dto.Password)));

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Name = dto.Name,
                Email = dto.Email,
                Role = dto.Role,
                PasswordHash = hash
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, new UserDto
            {
                UserId = user.UserId,
                Name = user.Name!,
                Email = user.Email!,
                Role = user.Role!
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            return new UserDto
            {
                UserId = user.UserId,
                Name = user.Name!,
                Email = user.Email!,
                Role = user.Role!
            };
        }

        [HttpGet]
        [Authorize(Roles = "Instructor")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            return await _context.Users
                .Select(user => new UserDto
                {
                    UserId = user.UserId,
                    Name = user.Name!,
                    Email = user.Email!,
                    Role = user.Role!
                })
                .ToListAsync();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Instructor,Student")]
        public async Task<IActionResult> PutUser(Guid id, [FromBody] UpdateUserDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            if (dto.Name != null) user.Name = dto.Name;
            if (dto.Email != null) user.Email = dto.Email;
            if (dto.Role != null) user.Role = dto.Role;

            if (dto.Password != null)
            {
                using var sha = System.Security.Cryptography.SHA256.Create();
                user.PasswordHash = Convert.ToBase64String(sha.ComputeHash(
                    System.Text.Encoding.UTF8.GetBytes(dto.Password)));
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
