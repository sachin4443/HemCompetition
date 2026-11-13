using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblRolePermission
{
    public int? GroupID { get; set; }

    public int? MenuId { get; set; }

    public bool? AddPer { get; set; }

    public bool? DelPer { get; set; }

    public bool? UpdPer { get; set; }

    public bool? EupdPer { get; set; }

    public bool? ViewPer { get; set; }
}
