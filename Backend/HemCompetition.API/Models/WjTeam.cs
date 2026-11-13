using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjTeam
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? Logo { get; set; }

    public string? Type { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? StatusDate { get; set; }

    public int? TeamCoins { get; set; }

    public virtual ICollection<WjTeamMember> WjTeamMembers { get; set; } = new List<WjTeamMember>();
}
