using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjTeacherAssociate
{
    public int TeacherId { get; set; }

    public int InstituteId { get; set; }

    public bool? IsPrimaryAssociate { get; set; }

    public virtual WjInstitute Institute { get; set; } = null!;

    public virtual WjTeacher Teacher { get; set; } = null!;
}
