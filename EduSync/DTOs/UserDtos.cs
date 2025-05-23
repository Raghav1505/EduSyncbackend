// DTOs/UserDtos.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace EduSync.DTOs
{
    /// <summary>
    /// Used when returning user data to clients (GET endpoints).
    /// </summary>
    public class UserDto
    {
        public Guid UserId { get; set; }
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Role { get; set; } = default!;
    }

    /// <summary>
    /// Used when creating a new user (POST).
    /// </summary>
    public class CreateUserDto
    {
        [Required, StringLength(100)]
        public string Name { get; set; } = default!;

        [Required, EmailAddress, StringLength(100)]
        public string Email { get; set; } = default!;

        [Required]
        public string Role { get; set; } = default!;

        [Required, MinLength(6)]
        public string Password { get; set; } = default!;  // plain-text; will be hashed server-side
    }

    /// <summary>
    /// Used when updating an existing user (PATCH/PUT).
    /// All properties are optional so you can update one or more fields.
    /// </summary>
    public class UpdateUserDto
    {
        [StringLength(100)]
        public string? Name { get; set; }

        [EmailAddress, StringLength(100)]
        public string? Email { get; set; }

        public string? Role { get; set; }

        [MinLength(6)]
        public string? Password { get; set; }  // if provided, will replace the existing hash
    }

    /// <summary>
    /// Optional: Used for login payloads.
    /// </summary>
}
