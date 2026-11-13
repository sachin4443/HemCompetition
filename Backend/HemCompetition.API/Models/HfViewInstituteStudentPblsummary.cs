using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfViewInstituteStudentPblsummary
{
    public int InstituteId { get; set; }

    public string Standard { get; set; } = null!;

    public int? TotalStudents { get; set; }

    public int? IndividualPblcompleted { get; set; }

    public int? FamilyPblcompleted { get; set; }

    public int? GroupPblcompleted { get; set; }
}
