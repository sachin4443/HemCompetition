using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class Transaction
{
    public int TransactionId { get; set; }

    public string TransactionNumber { get; set; } = null!;

    public DateTime TransactionDate { get; set; }

    public int AccountId { get; set; }

    public decimal? BasicAmount { get; set; }

    public string? TaxMode { get; set; }

    public int? TaxId { get; set; }

    public decimal? TaxAmount { get; set; }

    public string? DiscountType { get; set; }

    public string? DiscountMode { get; set; }

    public decimal? DiscountRate { get; set; }

    public decimal? DiscountAmt { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? TotalAmountInWords { get; set; }

    public string? Narration { get; set; }

    public bool? IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
