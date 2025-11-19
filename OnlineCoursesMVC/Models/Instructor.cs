using System;
using System.Collections.Generic;

namespace OnlineCoursesMVC.Models;

public partial class Instructor
{
    public int InstructorId { get; set; }

    public string FullName { get; set; } = null!;

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
