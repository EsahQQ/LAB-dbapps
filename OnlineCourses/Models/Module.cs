using System;
using System.Collections.Generic;

namespace OnlineCourses.Models;

public partial class Module
{
    public int ModuleId { get; set; }

    public int CourseId { get; set; }

    public string Title { get; set; } = null!;

    public int OrderNumber { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();
}
