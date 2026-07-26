using System;
using System.Collections.Generic;

namespace ProjectMarkStudentAPI.Models;

public partial class Subject
{
    public int SubjectId { get; set; }

    public string SubjectName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual ICollection<GradeItem> GradeItems { get; set; } = new List<GradeItem>();
}
