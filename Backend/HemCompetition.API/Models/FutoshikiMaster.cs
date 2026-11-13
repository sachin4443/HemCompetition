using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class FutoshikiMaster
{
    public int Fid { get; set; }

    public int? CourseId { get; set; }

    public string? Fsize { get; set; }

    public string? Fdifficulty { get; set; }

    public bool? Fisactive { get; set; }

    public string? FtopicName { get; set; }

    public virtual ICollection<FutoshikiSolution> FutoshikiSolutions { get; set; } = new List<FutoshikiSolution>();

    public virtual ICollection<FutoshikiTransaction> FutoshikiTransactions { get; set; } = new List<FutoshikiTransaction>();
}
