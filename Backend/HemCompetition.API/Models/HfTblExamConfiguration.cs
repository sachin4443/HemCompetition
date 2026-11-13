using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblExamConfiguration
{
    public int ConfigId { get; set; }

    public int InstituteId { get; set; }

    public int TermId { get; set; }

    public string Standard { get; set; } = null!;

    public int SubjectId { get; set; }

    public decimal MaxMarks { get; set; }

    public decimal PassingMarks { get; set; }

    public virtual ICollection<HfTblStudentMark> HfTblStudentMarks { get; set; } = new List<HfTblStudentMark>();

    public virtual WjInstitute Institute { get; set; } = null!;

    public virtual HfTblInstituteSubject Subject { get; set; } = null!;

    public virtual HfTblAcademicTerm Term { get; set; } = null!;
}
