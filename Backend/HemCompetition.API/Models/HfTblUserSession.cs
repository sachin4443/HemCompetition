using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblUserSession
{
    public int UserId { get; set; }

    public string? SessionId { get; set; }

    public DateTime? LastActivity { get; set; }
    public DateTime? LastHeartbeat { get; set; }

    public bool? IsActive { get; set; }
}
