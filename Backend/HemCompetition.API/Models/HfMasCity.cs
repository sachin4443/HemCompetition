using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfMasCity
{
    public int CityId { get; set; }

    public string? CityName { get; set; }

    public string? StateName { get; set; }

    public DateTime? EntDate { get; set; }
}
