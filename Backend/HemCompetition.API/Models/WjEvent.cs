using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjEvent
{
    public int EventId { get; set; }

    public int FkEventTypeId { get; set; }

    public string? Name { get; set; }

    public bool IsPaid { get; set; }

    public bool IsActive { get; set; }

    public string? Message { get; set; }

    public DateTime? CreatedOn { get; set; }

    public DateTime? EventStartDate { get; set; }

    public DateTime? EventEndDate { get; set; }

    public decimal? EventAmount { get; set; }

    public bool? Status { get; set; }

    public DateTime? EventParticipantStartDate { get; set; }

    public DateTime? EventParticipantEndDate { get; set; }

    public DateTime? EventMarketingEndDate { get; set; }

    public virtual WjEventType FkEventType { get; set; } = null!;
}
