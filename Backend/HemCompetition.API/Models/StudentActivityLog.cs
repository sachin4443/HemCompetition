using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class StudentActivityLog
{
    public int ActivityLogId { get; set; }

    public int StudentId { get; set; }

    public string ActivityDetails { get; set; } = null!;

    public int? Status { get; set; }

    public DateTime CreatedDate { get; set; }

    public bool IsChat { get; set; }
}
