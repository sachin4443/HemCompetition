using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblUserRole
{
    public int RoleId { get; set; }

    public int? AccessSiteId { get; set; }

    public string? Roles { get; set; }

    public int? UserTypeId { get; set; }

    public int? RecStatus { get; set; }
}
