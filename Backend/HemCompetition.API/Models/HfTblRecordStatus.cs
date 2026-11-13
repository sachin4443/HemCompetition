using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblRecordStatus
{
    public int RecStatus { get; set; }

    public string StatusName { get; set; } = null!;

    public string? StatusDescription { get; set; }
}
