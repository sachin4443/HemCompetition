using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class Game24Competition
{
    public int Gcid { get; set; }

    public string? GameType { get; set; }

    public string? GcLevel { get; set; }

    public string? Number1 { get; set; }

    public string? Number2 { get; set; }

    public string? Number3 { get; set; }

    public string? Number4 { get; set; }

    public string? Answer { get; set; }

    public DateTime? DisplayDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Game24UserSoln> Game24UserSolns { get; set; } = new List<Game24UserSoln>();
}
