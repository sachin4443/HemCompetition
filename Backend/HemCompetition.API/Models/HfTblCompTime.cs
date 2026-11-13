using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblCompTime
{
    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }
}
