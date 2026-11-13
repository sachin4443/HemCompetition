using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblStockTransfer
{
    public int TransferId { get; set; }

    public int? RequisitionId { get; set; }

    public int? TransferBy { get; set; }

    public DateTime? TransferDate { get; set; }

    public string? TransferMode { get; set; }

    public int? TransferQuantity { get; set; }

    public int? EntBy { get; set; }

    public DateTime? EntDate { get; set; }

    public int? UpdBy { get; set; }

    public DateTime? UpdDate { get; set; }

    public DateTime? DelDate { get; set; }

    public int? DelUserId { get; set; }

    public int? RecStatus { get; set; }
}
