using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class GuestsForEvent
{
    public int GuestId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? EmailId { get; set; }

    public string? MobileNo { get; set; }

    public string? Location { get; set; }

    public string? ReferredBy { get; set; }

    public DateTime? RegisteredDate { get; set; }
}
