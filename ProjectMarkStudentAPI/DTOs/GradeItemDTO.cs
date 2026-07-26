using System.ComponentModel.DataAnnotations;

namespace ProjectMarkStudentAPI.DTOs
{
    public class GradeItemDTO
    {
        public int GradeItemId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = null!;

        [Required]
        [Range(0, 100)]
        public decimal Rate { get; set; }

        [Required]
        public int SubjectId { get; set; }
    }
}
