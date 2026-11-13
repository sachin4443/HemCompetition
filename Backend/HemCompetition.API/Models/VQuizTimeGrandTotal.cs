using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class VQuizTimeGrandTotal
{
    public int UserId { get; set; }

    public TimeOnly? TotalSeconds { get; set; }
}
