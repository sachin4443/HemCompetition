using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class GetSelectedStandardList
{
    public int Id { get; set; }

    public string? Standards { get; set; }
}
