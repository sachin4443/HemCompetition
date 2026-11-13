using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfGameCountRecord
{
    public int GhfId { get; set; }

    public int GhfCount { get; set; }

    public int? GhfOver { get; set; }

    public string? GameName { get; set; }

    public int? UserId { get; set; }

    public DateTime? EntryDate { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
