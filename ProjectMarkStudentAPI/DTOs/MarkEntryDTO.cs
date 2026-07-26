using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectMarkStudentAPI.DTOs
{
    public class MarkEntryDTO
    {
        [Required]
        public int CourseId { get; set; }

        [Required]
        public int GradeItemId { get; set; }

        [Required]
        public Dictionary<int, decimal> StudentMarks { get; set; } = new Dictionary<int, decimal>();
    }
}
