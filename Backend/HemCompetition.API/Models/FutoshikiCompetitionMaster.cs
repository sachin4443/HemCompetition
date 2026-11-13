using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class FutoshikiCompetitionMaster
{
    public int Fcmid { get; set; }

    public string? GameType { get; set; }

    public string? Size { get; set; }

    public string? FcLevel { get; set; }

    public DateTime? DisplayDate { get; set; }

    public string? IsActive { get; set; }

    public virtual ICollection<FutoshikiCompetitionSolution> FutoshikiCompetitionSolutions { get; set; } = new List<FutoshikiCompetitionSolution>();

    public virtual ICollection<FutoshikiCompetitionTarget> FutoshikiCompetitionTargets { get; set; } = new List<FutoshikiCompetitionTarget>();

    public virtual ICollection<FutoshikiUserAnswer> FutoshikiUserAnswers { get; set; } = new List<FutoshikiUserAnswer>();
}
