using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjGift
{
    public int Id { get; set; }

    public int? StudentId { get; set; }

    public int FkWinnerId { get; set; }

    public string? GiftName { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? Type { get; set; }

    public bool? IsUsed { get; set; }

    public int? UsedCount { get; set; }

    public DateTime? ModifiedOn { get; set; }
}
