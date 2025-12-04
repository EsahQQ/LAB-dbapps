using Microsoft.EntityFrameworkCore;
using OnlineCoursesAPI.Models;

namespace OnlineCoursesAPI.Data;

public partial class Db31048Context : DbContext
{
    public Db31048Context()
    {
    }

    public Db31048Context(DbContextOptions<Db31048Context> options)
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

}
