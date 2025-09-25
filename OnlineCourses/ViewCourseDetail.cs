using System;
using System.Collections.Generic;

namespace OnlineCourses;

public partial class ViewCourseDetail
{
    public int CourseId { get; set; }

    public string CourseTitle { get; set; } = null!;

    public string? Description { get; set; }

    public string Category { get; set; } = null!;

    public string Difficulty { get; set; } = null!;

    public string Status { get; set; } = null!;

    public int InstructorId { get; set; }

    public string InstructorName { get; set; } = null!;
}
