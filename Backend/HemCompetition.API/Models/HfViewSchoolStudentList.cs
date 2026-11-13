using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfViewSchoolStudentList
{
    public int? StudentId { get; set; }

    public string? StudentName { get; set; }

    public string? City { get; set; }

    public string? Gender { get; set; }

    public string Standard { get; set; } = null!;

    public string? MobileNumber { get; set; }

    public string UserName { get; set; } = null!;

    public int? InstituteId { get; set; }

    public string Points { get; set; } = null!;
}
