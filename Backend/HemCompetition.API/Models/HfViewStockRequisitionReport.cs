using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfViewStockRequisitionReport
{
    public int RequisitionId { get; set; }

    public string? ItemType { get; set; }

    public int? UserId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? MobileNo { get; set; }

    public DateTime? RequestedDate { get; set; }

    public int? Quantity { get; set; }

    public string? Remarks { get; set; }

    public string? ShippingAddress { get; set; }

    public string? Status { get; set; }
}
