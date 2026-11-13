using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfViewCreativityStudentlist
{
    public int? StudentId { get; set; }

    public string? StudentName { get; set; }

    public string? City { get; set; }

    public string? MobileNumber { get; set; }

    public string? Gender { get; set; }

    public string Standard { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string? GroupName { get; set; }

    public string? ValueName { get; set; }

    public int? InstituteId { get; set; }
}
