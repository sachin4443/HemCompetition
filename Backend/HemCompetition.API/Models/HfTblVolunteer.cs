using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblVolunteer
{
    public int? VolunteerId { get; set; }

    public string? VolunteerType { get; set; }

    public string? ReferralCode { get; set; }

    public bool? IsActive { get; set; }
}
