using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfViewActiveTeacherUnderSchool
{
    public int InstituteId { get; set; }

    public int? TotalTeacher { get; set; }

    public int? ActiveTeacher { get; set; }

    public int? InActiveTeacher { get; set; }
}
