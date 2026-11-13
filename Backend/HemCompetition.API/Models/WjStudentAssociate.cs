using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjStudentAssociate
{
    public int StudentId { get; set; }

    public int InstituteId { get; set; }

    public bool? IsPrimaryAssociate { get; set; }

    public virtual WjInstitute Institute { get; set; } = null!;

    public virtual WjStudent Student { get; set; } = null!;
}
