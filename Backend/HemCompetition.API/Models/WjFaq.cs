using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjFaq
{
    public int Id { get; set; }

    public string Question { get; set; } = null!;

    public string? ImageMedia { get; set; }

    public string Answer { get; set; } = null!;

    public string? Category { get; set; }

    public bool? Status { get; set; }

    public DateTime? CreatedOn { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public bool? IsWhizjuniors { get; set; }

    public bool? IsTechathon { get; set; }

    public int? OrderNo { get; set; }
}
