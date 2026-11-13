using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblFailedUserRecord
{
    public int FailedRecordId { get; set; }

    public int? ApprovalId { get; set; }

    public string? RecordData { get; set; }

    public string Reason { get; set; } = null!;

    public DateTime Timestamp { get; set; }
}
