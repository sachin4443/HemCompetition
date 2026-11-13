using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblInstituteSubject
{
    public int SubjectId { get; set; }

    public int InstituteId { get; set; }

    public string SubjectName { get; set; } = null!;

    public string? SubjectCode { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<HfTblExamConfiguration> HfTblExamConfigurations { get; set; } = new List<HfTblExamConfiguration>();

    public virtual WjInstitute Institute { get; set; } = null!;
}
