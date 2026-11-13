using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfMonthlyBill
{
    public int SubmissionId { get; set; }

    public int? AffiliateId { get; set; }

    public string? FileActivity { get; set; }

    public string? ShortDescription { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public DateTime? AttendenceDate { get; set; }
}
