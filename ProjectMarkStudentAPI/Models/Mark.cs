using System;
using System.Collections.Generic;

namespace ProjectMarkStudentAPI.Models;

public partial class Mark
{
    public int MarkId { get; set; }

    public int StudentId { get; set; }

    public int CourseId { get; set; }

    public int GradeItemId { get; set; }

    public decimal Value { get; set; }

    public virtual GradeItem GradeItem { get; set; } = null!;

    public virtual StudentCourse StudentCourse { get; set; } = null!;
}
