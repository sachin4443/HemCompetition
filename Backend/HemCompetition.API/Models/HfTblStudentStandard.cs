using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblStudentStandard
{
    public int Id { get; set; }

    public int? StudentId { get; set; }

    public int? StdId { get; set; }

    public int? AcademicYearId { get; set; }

    public int? RecStatus { get; set; }

    public int? EntId { get; set; }

    public DateTime? EntDate { get; set; }
}
