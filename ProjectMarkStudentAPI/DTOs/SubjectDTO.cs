using System.ComponentModel.DataAnnotations;

namespace ProjectMarkStudentAPI.DTOs
{
    public class SubjectDTO
    {
        public int SubjectId { get; set; }

        [Required]
        [StringLength(100)]
        public string SubjectName { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }
    }
}
