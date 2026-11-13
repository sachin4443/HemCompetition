using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfAffiliateVisitEvent
{
    public int Id { get; set; }

    public int RecordId { get; set; }

    public DateTime EventDate { get; set; }

    public string? ColumnName { get; set; }

    public string? OriginalValue { get; set; }

    public string? NewValue { get; set; }

    public int? TenantId { get; set; }

    public int? AppUserId { get; set; }

    public string? TableName { get; set; }
}
