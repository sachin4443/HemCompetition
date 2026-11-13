using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfViewInstitutewiseTopStudentPerformer
{
    public int? InstituteId { get; set; }

    public string? Standard { get; set; }

    public int? StudentId { get; set; }

    public string StudentName { get; set; } = null!;

    public int? TotalPoints { get; set; }

    public long? Ranks { get; set; }

    public long? InstRank { get; set; }
}
