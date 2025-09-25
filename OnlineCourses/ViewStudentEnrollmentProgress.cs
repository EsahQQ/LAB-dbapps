using System;
using System.Collections.Generic;

namespace OnlineCourses;

public partial class ViewStudentEnrollmentProgress
{
    public int EnrollmentId { get; set; }

    public int StudentId { get; set; }

    public string StudentName { get; set; } = null!;

    public string StudentEmail { get; set; } = null!;

    public int CourseId { get; set; }

    public string CourseTitle { get; set; } = null!;

    public string CourseCategory { get; set; } = null!;

    public decimal Progress { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }
}
