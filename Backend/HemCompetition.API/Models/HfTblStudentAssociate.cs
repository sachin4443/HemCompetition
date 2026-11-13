using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblStudentAssociate
{
    public int? StudentId { get; set; }

    public int? InstituteId { get; set; }

    public bool? IsPrimaryAssociate { get; set; }

    public int? RecStatus { get; set; }

    public int? EntId { get; set; }

    public DateTime? EntDate { get; set; }
}
