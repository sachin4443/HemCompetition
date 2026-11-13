using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfSchoolVisit
{
    public int VisitId { get; set; }

    public DateOnly? VisitedDate { get; set; }

    public int? EntBy { get; set; }

    public DateTime? EndDate { get; set; }

    public int? RecStatus { get; set; }

    public int? UpdBy { get; set; }

    public DateTime? UpdDate { get; set; }

    public int? DelBy { get; set; }

    public DateTime? DelDate { get; set; }

    public int? InstituteId { get; set; }

    public int? ScheduleId { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }
}
