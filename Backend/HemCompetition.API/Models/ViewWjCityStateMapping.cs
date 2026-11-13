using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class ViewWjCityStateMapping
{
    public int FkCityId { get; set; }

    public int FkStateId { get; set; }

    public string? CityName { get; set; }

    public string? StateName { get; set; }
}
