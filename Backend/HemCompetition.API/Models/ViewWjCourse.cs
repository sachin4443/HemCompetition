using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class ViewWjCourse
{
    public int CourseId { get; set; }

    public string CourseName { get; set; } = null!;

    public string? Description { get; set; }

    public string? CourseCategorys { get; set; }

    public int? FromAge { get; set; }

    public int? ToAge { get; set; }
}
