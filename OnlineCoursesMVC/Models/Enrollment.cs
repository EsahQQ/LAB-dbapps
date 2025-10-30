using System;
using System.Collections.Generic;

namespace OnlineCoursesMVC.Models;

public partial class Enrollment
{
    public int EnrollmentId { get; set; }

    public int StudentId { get; set; }

    public int CourseId { get; set; }

    public decimal Progress { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
