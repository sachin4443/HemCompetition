using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfViewStudentsDetail
{
    public int StudentId { get; set; }

    public string StudentName { get; set; } = null!;

    public int? InstituteId { get; set; }

    public string? InstituteName { get; set; }

    public string Standard { get; set; } = null!;

    public string? Div { get; set; }

    public string? Gender { get; set; }

    public string? CityName { get; set; }

    public string? Pincode { get; set; }

    public int TotalMarksGained { get; set; }

    public int NoOfCoursesCompleted { get; set; }

    public string? Address { get; set; }

    public string? MobileNo { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public long? Rank { get; set; }
}
