using System;
using System.Collections.Generic;

namespace ProjectMarkStudentAPI.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Gender { get; set; }

    public string? AvatarUrl { get; set; }

    public string PasswordHash { get; set; } = null!;

    public int RoleId { get; set; }

    public bool Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual Role Role { get; set; } = null!;
}
