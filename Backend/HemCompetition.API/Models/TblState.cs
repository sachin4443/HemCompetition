using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class TblState
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int CountryId { get; set; }
}
