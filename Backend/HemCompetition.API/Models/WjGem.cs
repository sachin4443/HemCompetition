using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjGem
{
    public int GemsId { get; set; }

    public int StudentId { get; set; }

    public string? GemsFor { get; set; }

    public int? ReferenceId { get; set; }

    public int? Gems { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? QuizLevelId { get; set; }

    public bool? Status { get; set; }

    public bool? IsApp { get; set; }
}
