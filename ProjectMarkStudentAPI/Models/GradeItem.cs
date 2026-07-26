using System;
using System.Collections.Generic;

namespace ProjectMarkStudentAPI.Models;

public partial class GradeItem
{
    public int GradeItemId { get; set; }

    public string Title { get; set; } = null!;

    public decimal Rate { get; set; }

    public int SubjectId { get; set; }

    public virtual ICollection<Mark> Marks { get; set; } = new List<Mark>();

    public virtual Subject Subject { get; set; } = null!;
}
