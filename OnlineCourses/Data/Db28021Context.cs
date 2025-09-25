using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.SqlClient;
using OnlineCourses.Models;

namespace OnlineCourses.Data;

public partial class Db28021Context : DbContext
{
    public Db28021Context()
    {
    }

    public Db28021Context(DbContextOptions<Db28021Context> options)
        : base(options)
    {
    }


    public virtual DbSet<Certificate> Certificates { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Enrollment> Enrollments { get; set; }

    public virtual DbSet<Instructor> Instructors { get; set; }

    public virtual DbSet<Module> Modules { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Test> Tests { get; set; }

    public virtual DbSet<TestResult> TestResults { get; set; }

    public virtual DbSet<ViewCourseDetail> ViewCourseDetails { get; set; }

    public virtual DbSet<ViewFullTestResult> ViewFullTestResults { get; set; }

    public virtual DbSet<ViewStudentEnrollmentProgress> ViewStudentEnrollmentProgresses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        ConfigurationBuilder builder = new();

        builder.SetBasePath(Directory.GetCurrentDirectory());

        builder.AddJsonFile("appsettings.json");

        IConfigurationRoot configuration = builder.AddUserSecrets<Program>().Build();

        string connectionString = "";

        string secretPass = configuration["Database:password"];
        string secretUser = configuration["Database:login"];
        SqlConnectionStringBuilder sqlConnectionStringBuilder = new(configuration.GetConnectionString("RemoteSQLConnection"))
        {
            Password = secretPass,
            UserID= secretUser
        };
        connectionString = sqlConnectionStringBuilder.ConnectionString;

        _ = optionsBuilder
            .UseSqlServer(connectionString)
            .Options;
        optionsBuilder.LogTo(message => System.Diagnostics.Debug.WriteLine(message));
    }
           

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Cyrillic_General_CI_AS");

        modelBuilder.Entity<Certificate>(entity =>
        {
            entity.HasKey(e => e.CertificateId).HasName("PK__Certific__BBF8A7E13B35CC2E");

            entity.HasIndex(e => e.CertificateUrl, "UQ__Certific__D5D4A4C68BB0CB7A").IsUnique();

            entity.Property(e => e.CertificateId).HasColumnName("CertificateID");
            entity.Property(e => e.CertificateUrl).HasMaxLength(255);
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.Course).WithMany(p => p.Certificates)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK_Certificates_Courses");

            entity.HasOne(d => d.Student).WithMany(p => p.Certificates)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK_Certificates_Students");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Courses__C92D7187A303C518");

            entity.HasIndex(e => e.Title, "UQ_Courses_Title").IsUnique();

            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.Difficulty).HasMaxLength(50);
            entity.Property(e => e.InstructorId).HasColumnName("InstructorID");
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Instructor).WithMany(p => p.Courses)
                .HasForeignKey(d => d.InstructorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Courses_Instructors");
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.EnrollmentId).HasName("PK__Enrollme__7F6877FB9FF867A0");

            entity.HasIndex(e => new { e.StudentId, e.CourseId }, "UQ_Student_Course").IsUnique();

            entity.Property(e => e.EnrollmentId).HasColumnName("EnrollmentID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.Progress)
                .HasDefaultValueSql("((0.00))")
                .HasColumnType("decimal(5, 2)");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.Course).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK_Enrollments_Courses");

            entity.HasOne(d => d.Student).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK_Enrollments_Students");
        });

        modelBuilder.Entity<Instructor>(entity =>
        {
            entity.HasKey(e => e.InstructorId).HasName("PK__Instruct__9D010B7B91B23335");

            entity.Property(e => e.InstructorId).HasColumnName("InstructorID");
            entity.Property(e => e.FullName).HasMaxLength(150);
        });

        modelBuilder.Entity<Module>(entity =>
        {
            entity.HasKey(e => e.ModuleId).HasName("PK__Modules__2B74778799D297BB");

            entity.Property(e => e.ModuleId).HasColumnName("ModuleID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Course).WithMany(p => p.Modules)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK_Modules_Courses");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Students__32C52A79D97AAA29");

            entity.HasIndex(e => e.Email, "UQ__Students__A9D10534FE129239").IsUnique();

            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.RegistrationDate).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<Test>(entity =>
        {
            entity.HasKey(e => e.TestId).HasName("PK__Tests__8CC331008A64EE37");

            entity.Property(e => e.TestId).HasColumnName("TestID");
            entity.Property(e => e.ModuleId).HasColumnName("ModuleID");
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Module).WithMany(p => p.Tests)
                .HasForeignKey(d => d.ModuleId)
                .HasConstraintName("FK_Tests_Modules");
        });

        modelBuilder.Entity<TestResult>(entity =>
        {
            entity.HasKey(e => e.ResultId).HasName("PK__TestResu__97690228F14CB880");

            entity.Property(e => e.ResultId).HasColumnName("ResultID");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.TestId).HasColumnName("TestID");

            entity.HasOne(d => d.Student).WithMany(p => p.TestResults)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK_TestResults_Students");

            entity.HasOne(d => d.Test).WithMany(p => p.TestResults)
                .HasForeignKey(d => d.TestId)
                .HasConstraintName("FK_TestResults_Tests");
        });

        modelBuilder.Entity<ViewCourseDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_CourseDetails");

            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CourseTitle).HasMaxLength(200);
            entity.Property(e => e.Difficulty).HasMaxLength(50);
            entity.Property(e => e.InstructorId).HasColumnName("InstructorID");
            entity.Property(e => e.InstructorName).HasMaxLength(150);
            entity.Property(e => e.Status).HasMaxLength(20);
        });

        modelBuilder.Entity<ViewFullTestResult>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_FullTestResults");

            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CourseTitle).HasMaxLength(200);
            entity.Property(e => e.ModuleId).HasColumnName("ModuleID");
            entity.Property(e => e.ModuleTitle).HasMaxLength(200);
            entity.Property(e => e.ResultId).HasColumnName("ResultID");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.StudentName).HasMaxLength(150);
            entity.Property(e => e.TestId).HasColumnName("TestID");
            entity.Property(e => e.TestTitle).HasMaxLength(200);
        });

        modelBuilder.Entity<ViewStudentEnrollmentProgress>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_StudentEnrollmentProgress");

            entity.Property(e => e.CourseCategory).HasMaxLength(100);
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CourseTitle).HasMaxLength(200);
            entity.Property(e => e.EnrollmentId).HasColumnName("EnrollmentID");
            entity.Property(e => e.Progress).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.StudentEmail).HasMaxLength(100);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.StudentName).HasMaxLength(150);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
