using System;
using System.Collections.Generic;

namespace OnlineCourses;

public partial class ViewFullTestResult
{
    public long ResultId { get; set; }

    public int StudentId { get; set; }

    public string StudentName { get; set; } = null!;

    public int CourseId { get; set; }

    public string CourseTitle { get; set; } = null!;

    public int ModuleId { get; set; }

    public string ModuleTitle { get; set; } = null!;

    public int TestId { get; set; }

    public string TestTitle { get; set; } = null!;

    public int Score { get; set; }

    public DateOnly CompletionDate { get; set; }
}
