using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjCity
{
    public int CityId { get; set; }

    public string CityName { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public virtual ICollection<WjZoneWise> WjZoneWises { get; set; } = new List<WjZoneWise>();
}
