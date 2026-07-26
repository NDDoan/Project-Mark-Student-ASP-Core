using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectMarkStudentAPI.DTOs
{
    public class StudentDTO
    {
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100)]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Roll number is required")]
        [StringLength(8)]
        public string RollNumber { get; set; } = null!;

        [Required(ErrorMessage = "First name is required")]
        [StringLength(100)]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100)]
        public string LastName { get; set; } = null!;

        public DateOnly? Dob { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }
    }
}
