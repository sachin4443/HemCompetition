using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfViewInstituteTeacherList
{
    public int? TeacherId { get; set; }

    public string? TeacherName { get; set; }

    public string? InstituteName { get; set; }

    public string? Standard { get; set; }

    public string? City { get; set; }

    public string? Gender { get; set; }

    public string? MobileNumber { get; set; }

    public string UserName { get; set; } = null!;

    public int? InstituteId { get; set; }

    public DateTime RegisteredDate { get; set; }
}
