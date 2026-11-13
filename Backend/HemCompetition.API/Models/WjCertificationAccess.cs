using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjCertificationAccess
{
    public int VipId { get; set; }

    public string? Name { get; set; }

    public string? ImageUrl { get; set; }

    public string? StartFrom { get; set; }

    public string? EndtO { get; set; }

    public int CertificationCount { get; set; }

    public bool? Status { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int CoinsCount { get; set; }

    public int GemsEarned { get; set; }

    public virtual ICollection<WjStudentCertificationAccess> WjStudentCertificationAccesses { get; set; } = new List<WjStudentCertificationAccess>();
}
