namespace ProjectMarkStudentAPI.DTOs
{
    /// <summary>
    /// DTO returned for individual Mark records — avoids exposing navigation properties
    /// from the raw Mark entity (Issue #4).
    /// </summary>
    public class MarkDTO
    {
        public int MarkId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public int GradeItemId { get; set; }
        public string GradeItemTitle { get; set; } = null!;
        public decimal Value { get; set; }
    }
}
