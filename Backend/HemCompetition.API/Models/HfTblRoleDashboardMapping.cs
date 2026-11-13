using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblRoleDashboardMapping
{
    public int RoleDashboardId { get; set; }

    public int RoleId { get; set; }

    public string DashboardName { get; set; } = null!;

    public string DashboardUrl { get; set; } = null!;

    public string? Description { get; set; }

    public int? RecStatus { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }
}
