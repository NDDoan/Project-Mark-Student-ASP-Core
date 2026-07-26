using System.ComponentModel.DataAnnotations;

namespace ProjectMarkStudentAPI.DTOs
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = null!;
    }
}
