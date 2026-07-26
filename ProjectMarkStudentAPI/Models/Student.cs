using System;
using System.Collections.Generic;

namespace ProjectMarkStudentAPI.Models;

public partial class Student
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string RollNumber { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly? Dob { get; set; }

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Gender { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
}
