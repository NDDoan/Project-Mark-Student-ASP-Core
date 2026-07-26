using System.ComponentModel.DataAnnotations;

namespace ProjectMarkStudentAPI.DTOs
{
    public class CourseDTO
    {
        public int CourseId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = null!;

        [Required]
        public int SubjectId { get; set; }

        public string? SubjectName { get; set; }

        public int? TeacherId { get; set; }

        public string? TeacherName { get; set; }
    }
}
