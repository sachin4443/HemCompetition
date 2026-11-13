using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblStockRequisition
{
    public int RequisitionId { get; set; }

    public string? ItemType { get; set; }

    public int? RequestedBy { get; set; }

    public DateTime? RequestedDate { get; set; }

    public int? Quantity { get; set; }

    public string? Status { get; set; }

    public int? EntBy { get; set; }

    public DateTime? EntDate { get; set; }

    public int? UpdBy { get; set; }

    public DateTime? UpdDate { get; set; }

    public DateTime? DelDate { get; set; }

    public int? DelUserId { get; set; }

    public int? RecStatus { get; set; }

    public string? Remarks { get; set; }

    public string? ShippingAddress { get; set; }
}
