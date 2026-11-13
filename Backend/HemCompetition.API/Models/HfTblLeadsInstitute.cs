using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblLeadsInstitute
{
    public int LeadsId { get; set; }

    public string? InstituteName { get; set; }

    public string? ContactPerson { get; set; }

    public string? MobileNo { get; set; }

    public string? Designation { get; set; }

    public string? Feedback { get; set; }

    public string? Address { get; set; }

    public int? Pincode { get; set; }

    public int? AffiliateId { get; set; }

    public int? EntId { get; set; }

    public DateTime? EntDate { get; set; }

    public int? RecStatus { get; set; }

    public int? UpdId { get; set; }

    public DateTime? UpdDate { get; set; }

    public int? LeadsConverted { get; set; }

    public DateOnly? LeadsConversionDate { get; set; }

    public string? LeadsSource { get; set; }

    public string? Address3 { get; set; }

    public string? City { get; set; }

    public string? DocumentPath { get; set; }

    public string? EmailId { get; set; }
}
