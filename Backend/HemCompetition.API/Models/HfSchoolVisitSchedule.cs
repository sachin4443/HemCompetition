using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfSchoolVisitSchedule
{
    public int ScheduleId { get; set; }

    public DateTime? ScheduleDate { get; set; }

    public int? EntBy { get; set; }

    public DateTime? EndDate { get; set; }

    public int? RecStatus { get; set; }

    public int? UpdBy { get; set; }

    public DateTime? UpdDate { get; set; }

    public int? DelBy { get; set; }

    public DateTime? DelDate { get; set; }

    public int? InstituteId { get; set; }

    public bool? Isvisited { get; set; }

    public DateTime? VisitedDate { get; set; }

    public DateTime? VisitedBy { get; set; }

    public bool? IsvisitCancelled { get; set; }

    public string? CancelledReason { get; set; }

    public string? Purpose { get; set; }
}
