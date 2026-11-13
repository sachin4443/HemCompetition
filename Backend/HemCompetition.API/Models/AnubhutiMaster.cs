using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class AnubhutiMaster
{
    public int AnbId { get; set; }

    public DateTime? EntryDate { get; set; }

    public int? CourseId { get; set; }

    public string? AnbType { get; set; }

    public string? AnbQuestion { get; set; }

    public string? AnbAnswer { get; set; }

    public bool? AnbTrue { get; set; }

    public virtual ICollection<AnubhutiTransaction> AnubhutiTransactions { get; set; } = new List<AnubhutiTransaction>();
}
