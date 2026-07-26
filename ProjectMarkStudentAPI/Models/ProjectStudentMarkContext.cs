using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ProjectMarkStudentAPI.Models;

public partial class ProjectStudentMarkContext : DbContext
{
    public ProjectStudentMarkContext()
    {
    }

    public ProjectStudentMarkContext(DbContextOptions<ProjectStudentMarkContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<GradeItem> GradeItems { get; set; }

    public virtual DbSet<Mark> Marks { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentCourse> StudentCourses { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        optionsBuilder.UseSqlServer(config.GetConnectionString("MyCnn"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Courses__C92D71A722E9F6F6");

            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.Subject).WithMany(p => p.Courses)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Course_Subject");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Courses)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK_Course_Teacher");
        });

        modelBuilder.Entity<GradeItem>(entity =>
        {
            entity.HasKey(e => e.GradeItemId).HasName("PK__GradeIte__A40A40361FBC6B9C");

            entity.Property(e => e.Rate).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.Subject).WithMany(p => p.GradeItems)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GradeItem_Subject");
        });

        modelBuilder.Entity<Mark>(entity =>
        {
            entity.HasKey(e => e.MarkId).HasName("PK__Marks__4E30D366C0BE9587");

            entity.HasIndex(e => new { e.StudentId, e.CourseId, e.GradeItemId }, "UQ__Marks__69F3F6C23F4C47E3").IsUnique();

            entity.Property(e => e.Value).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.GradeItem).WithMany(p => p.Marks)
                .HasForeignKey(d => d.GradeItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Mark_GradeItem");

            entity.HasOne(d => d.StudentCourse).WithMany(p => p.Marks)
                .HasForeignKey(d => new { d.StudentId, d.CourseId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Mark_StudentCourse");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1A85828A51");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B61604D47F2C0").IsUnique();

            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Students__3214EC0722EF27E7");

            entity.HasIndex(e => e.Email, "UQ__Students__A9D105340FBE4CB9").IsUnique();

            entity.HasIndex(e => e.RollNumber, "UQ__Students__E9F06F16913E41A1").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.RollNumber).HasMaxLength(8);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<StudentCourse>(entity =>
        {
            entity.HasKey(e => new { e.StudentId, e.CourseId }).HasName("PK__StudentC__5E57FC83A69162C6");

            entity.HasOne(d => d.Course).WithMany(p => p.StudentCourses)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentCourse_Course");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentCourses)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentCourse_Student");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.SubjectId).HasName("PK__Subjects__AC1BA3A8F5EAC07C");

            entity.HasIndex(e => e.SubjectName, "UQ__Subjects__4C5A7D55E87C4FEF").IsUnique();

            entity.Property(e => e.SubjectName).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CBD78C642");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E47F3304C1").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.AvatarUrl).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
