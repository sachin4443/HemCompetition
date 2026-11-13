using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjStudentCertificationAccess
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public int VipId { get; set; }

    public bool? Status { get; set; }

    public DateTime? CreatedOn { get; set; }

    public virtual WjCertificationAccess Vip { get; set; } = null!;
}
