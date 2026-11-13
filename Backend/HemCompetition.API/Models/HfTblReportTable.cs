using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblReportTable
{
    public int Id { get; set; }

    public string? ViewName { get; set; }

    public string? ProcedureName { get; set; }

    public string? Description { get; set; }
}
