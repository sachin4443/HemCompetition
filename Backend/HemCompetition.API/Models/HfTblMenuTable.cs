using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblMenuTable
{
    public int TableId { get; set; }

    public string? TableName { get; set; }

    public string? TableDesc { get; set; }

    public int? MenuId { get; set; }

    public string? ApvlId1Column { get; set; }

    public string? ApvlId2Column { get; set; }

    public string? ApvlId3Column { get; set; }

    public string? ApvlStatus1Column { get; set; }

    public string? ApvlStatus2Column { get; set; }

    public string? ApvlStatus3Column { get; set; }

    public string? ApvlRemarks1Column { get; set; }

    public string? ApvlRemarks2Column { get; set; }

    public string? ApvlRemarks3Column { get; set; }

    public string? StatusColumn { get; set; }

    public string? ApvlDate { get; set; }

    public string? ApvlDate2 { get; set; }

    public string? ApvlDate3 { get; set; }

    public string? KeyIdColumn { get; set; }

    public string? DelDateColumn { get; set; }

    public string? DelUserIdColumn { get; set; }
}
