using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblAcademicTerm
{
    public int TermId { get; set; }

    public int InstituteId { get; set; }

    public string TermName { get; set; } = null!;

    public string AcademicYear { get; set; } = null!;

    public bool? IsActive { get; set; }

    public virtual ICollection<HfTblExamConfiguration> HfTblExamConfigurations { get; set; } = new List<HfTblExamConfiguration>();

    public virtual WjInstitute Institute { get; set; } = null!;
}
