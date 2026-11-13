using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfViewValuewisePassTeacher
{
    public int? TeacherId { get; set; }

    public string TeacherName { get; set; } = null!;

    public int? InstituteId { get; set; }

    public string? InstituteName { get; set; }

    public string? MainCourseName { get; set; }

    public int? Points { get; set; }
}
