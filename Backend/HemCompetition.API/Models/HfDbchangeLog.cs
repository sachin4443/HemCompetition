using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfDbchangeLog
{
    public int DbchangeLogId { get; set; }

    public string DatabaseName { get; set; } = null!;

    public string EventType { get; set; } = null!;

    public string ObjectName { get; set; } = null!;

    public string ObjectType { get; set; } = null!;

    public string SqlCommand { get; set; } = null!;

    public DateTime EventDate { get; set; }

    public string LoginName { get; set; } = null!;
}
