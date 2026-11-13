using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblEmailReminderLog
{
    public int LogId { get; set; }

    public int? StudentId { get; set; }

    public string? CourseName { get; set; }

    public DateTime? SentDate { get; set; }
}
