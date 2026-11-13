using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class KnowlegdePartnersList
{
    public int Kpid { get; set; }

    public int? InstituteId { get; set; }

    public string? InstituteName { get; set; }

    public string? FileUrl { get; set; }
}
