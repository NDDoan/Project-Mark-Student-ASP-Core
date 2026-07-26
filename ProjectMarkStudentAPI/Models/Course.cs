    using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectMarkStudentAPI.Models;

public partial class Course
{
    public int CourseId { get; set; }

    public string Title { get; set; } = null!;

    public int SubjectId { get; set; }

    public int? TeacherId { get; set; }

    public DateOnly? StartDate { get; set; }

    public virtual ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();

    public virtual Subject Subject { get; set; } = null!;

    public virtual User? Teacher { get; set; }
}
