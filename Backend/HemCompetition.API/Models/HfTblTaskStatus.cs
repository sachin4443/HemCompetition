using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblTaskStatus
{
    public int StatusId { get; set; }

    public string StatusName { get; set; } = null!;
}
