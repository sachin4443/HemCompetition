using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HvCompetitionConfirmation
{
    public int CompId { get; set; }

    public int? InstituteId { get; set; }

    public string? CompetitionSelected { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
