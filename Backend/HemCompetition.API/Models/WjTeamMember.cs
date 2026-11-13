using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjTeamMember
{
    public int Id { get; set; }

    public int FkTeamId { get; set; }

    public int StudentId { get; set; }

    public bool? IsLeader { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? Message { get; set; }

    public DateTime? StatusDate { get; set; }

    public string Status { get; set; } = null!;

    public int? TeacherId { get; set; }

    public virtual WjTeam FkTeam { get; set; } = null!;
}
