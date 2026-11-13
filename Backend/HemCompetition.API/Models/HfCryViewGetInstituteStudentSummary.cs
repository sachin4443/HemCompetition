using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfCryViewGetInstituteStudentSummary
{
    public int InstituteId { get; set; }

    public int? TotalStudents { get; set; }

    public int ActiveStudents { get; set; }

    public int? InactiveStudents { get; set; }
}
