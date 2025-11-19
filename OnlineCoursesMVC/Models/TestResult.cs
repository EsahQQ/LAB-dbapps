using System;
using System.Collections.Generic;

namespace OnlineCoursesMVC.Models;

public partial class TestResult
{
    public long TestResultId { get; set; }

    public int StudentId { get; set; }

    public int TestId { get; set; }

    public int Score { get; set; }

    public DateOnly CompletionDate { get; set; }

    public virtual Student Student { get; set; } = null!;

    public virtual Test Test { get; set; } = null!;
}
