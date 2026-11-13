using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblAcademicYear
{
    public int AcademicYearId { get; set; }

    public string? YearLabel { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public bool? IsCurrent { get; set; }
}
