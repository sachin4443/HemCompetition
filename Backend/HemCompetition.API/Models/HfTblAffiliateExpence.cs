using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblAffiliateExpence
{
    public int ExpencesId { get; set; }

    public DateTime? ExpencesDate { get; set; }

    public decimal? ExpencesAmount { get; set; }

    public int? VisitId { get; set; }

    public int? InstituteId { get; set; }

    public string? FromLocation { get; set; }

    public string? ToLocation { get; set; }

    public int? ExpencesType { get; set; }

    public string? TravelMode { get; set; }

    public decimal? ApprovedAmount { get; set; }

    public int? PaymentStatus { get; set; }

    public DateTime? PaymentDate { get; set; }

    public int? PaymentBy { get; set; }

    public decimal? PaymentAmount { get; set; }

    public int? RecStatus { get; set; }

    public int? ApvlId1 { get; set; }

    public int? ApvlId2 { get; set; }

    public int? ApvlId3 { get; set; }

    public int? ApvlStatus1 { get; set; }

    public int? ApvlStatus2 { get; set; }

    public int? ApvlStatus3 { get; set; }

    public int? ApvlRemarks1 { get; set; }

    public int? ApvlRemarks2 { get; set; }

    public int? ApvlRemarks3 { get; set; }

    public int? ApvlDate { get; set; }

    public int? ApvlDate2 { get; set; }

    public int? ApvlDate3 { get; set; }

    public DateTime? DelDate { get; set; }

    public int? DelUserId { get; set; }

    public int? EntBy { get; set; }

    public DateTime? EntDate { get; set; }

    public int? UpdBy { get; set; }

    public DateTime? UpdDate { get; set; }

    public string? AttachmentPath { get; set; }
}
