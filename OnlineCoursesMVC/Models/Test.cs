using System;
using System.Collections.Generic;

namespace OnlineCoursesMVC.Models;

public partial class Test
{
    public int TestId { get; set; }

    public int ModuleId { get; set; }

    public string Title { get; set; } = null!;

    public virtual Module Module { get; set; } = null!;

    public virtual ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();
}
