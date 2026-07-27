using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectMarkStudentAPI.DTOs
{
    public class UserDTO
    {
        public int UserId { get; set; }

        public string Username { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public string? AvatarUrl { get; set; }

        public string? Address { get; set; }

        public string? Gender { get; set; }

        public int RoleId { get; set; }

        public string? RoleName { get; set; }
    }

    public class CreateUserDTO
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string Password { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = null!;

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [StringLength(500)]
        public string? AvatarUrl { get; set; }
        
        [StringLength(255)]
        public string? Address { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }

        [Required]
        public int RoleId { get; set; }
    }

    public class UpdateUserDTO
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = null!;

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [StringLength(500)]
        public string? AvatarUrl { get; set; }
        
        [StringLength(255)]
        public string? Address { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }

        [Required]
        public int RoleId { get; set; }
    }

    public class ChangePasswordDTO
    {
        [Required]
        [StringLength(255)]
        public string OldPassword { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string NewPassword { get; set; } = null!;
    }
}
