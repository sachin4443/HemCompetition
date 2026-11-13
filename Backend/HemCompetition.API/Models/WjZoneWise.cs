using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjZoneWise
{
    public int Id { get; set; }

    public int FkCityId { get; set; }

    public int FkStateId { get; set; }

    public int FkZoneId { get; set; }

    public DateTime? CreatedOn { get; set; }

    public bool Status { get; set; }

    public virtual WjCity FkCity { get; set; } = null!;

    public virtual WjState FkState { get; set; } = null!;
}
