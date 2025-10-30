using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.SqlClient;
using OnlineCoursesMVC.Models;

namespace OnlineCoursesMVC.Data;

public partial class Db31048Context : DbContext
{
    public Db31048Context()
    {
    }

    public Db31048Context(DbContextOptions<Db31048Context> options)
        : base(options)
    {
    }


    public DbSet<Certificate> Certificates { get; set; }

    public DbSet<Course> Courses { get; set; }

    public DbSet<Enrollment> Enrollments { get; set; }

    public DbSet<Instructor> Instructors { get; set; }

    public DbSet<Module> Modules { get; set; }

    public DbSet<Student> Students { get; set; }

    public DbSet<Test> Tests { get; set; }

    public DbSet<TestResult> TestResults { get; set; }

}
