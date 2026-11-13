using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblUserSessionsAson21122025
{
    public int UserId { get; set; }

    public string? SessionId { get; set; }

    public DateTime? LastActivity { get; set; }

    public bool? IsActive { get; set; }
}
