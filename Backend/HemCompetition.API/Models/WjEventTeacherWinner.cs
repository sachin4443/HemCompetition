using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjEventTeacherWinner
{
    public int Id { get; set; }

    public int FkEventIddetails { get; set; }

    public int? TeacherId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? InstituteName { get; set; }

    public string? Address { get; set; }

    public string? EmailId { get; set; }

    public string? MobileNo { get; set; }

    public DateTime? WinningDate { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? Coins { get; set; }

    public int? Rank { get; set; }
}
