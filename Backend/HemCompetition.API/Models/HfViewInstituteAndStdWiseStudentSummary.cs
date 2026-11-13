using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfViewInstituteAndStdWiseStudentSummary
{
    public string? Name { get; set; }

    public int InstituteId { get; set; }

    public string Standard { get; set; } = null!;

    public int? TotalStudents { get; set; }

    public int? ActiveStudents { get; set; }

    public int? InactiveStudents { get; set; }
}
