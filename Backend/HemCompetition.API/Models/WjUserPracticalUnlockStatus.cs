using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjUserPracticalUnlockStatus
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? PracticalId { get; set; }

    public int? GemsSpent { get; set; }

    public bool? IsTask { get; set; }

    public bool? Status { get; set; }

    public DateTime? CreatedOn { get; set; }
}
