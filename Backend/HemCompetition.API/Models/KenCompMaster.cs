using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class KenCompMaster
{
    public int Kcmid { get; set; }

    public string? QuizType { get; set; }

    public string? Size { get; set; }

    public string? Operation { get; set; }

    public string? KenLevel { get; set; }

    public DateTime? DisplayDate { get; set; }

    public string? IsActive { get; set; }

    public virtual ICollection<KenCompCage> KenCompCages { get; set; } = new List<KenCompCage>();

    public virtual ICollection<KenCompSolution> KenCompSolutions { get; set; } = new List<KenCompSolution>();

    public virtual ICollection<KenCompTarget> KenCompTargets { get; set; } = new List<KenCompTarget>();
}
