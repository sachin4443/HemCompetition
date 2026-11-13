using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjState
{
    public int StateId { get; set; }

    public string StateName { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public virtual ICollection<WjZoneWise> WjZoneWises { get; set; } = new List<WjZoneWise>();
}
