using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjReceiptVoucher
{
    public int ReceiptVoucherId { get; set; }

    public string RcptNumber { get; set; } = null!;

    public DateTime RcptDate { get; set; }

    public string RcptMode { get; set; } = null!;

    public string? ChqBankName { get; set; }

    public string? ChqNumber { get; set; }

    public DateTime? ChqDate { get; set; }

    public string? ReferenceNumber { get; set; }

    public decimal RcptAmount { get; set; }

    public string? RcptAmountInWords { get; set; }

    public string? Narration { get; set; }

    public string? Remark { get; set; }

    public bool? IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
