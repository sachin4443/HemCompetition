using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblVolunteerSchoolAssociation
{
    public int Id { get; set; }

    public int? VolunteerId { get; set; }

    public int? SchoolId { get; set; }

    public int? RecStatus { get; set; }

    public int? EntBy { get; set; }

    public DateTime? EntDatetime { get; set; }
}
