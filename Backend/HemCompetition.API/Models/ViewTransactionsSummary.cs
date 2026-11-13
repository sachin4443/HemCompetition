using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class ViewTransactionsSummary
{
    public long? RowNo { get; set; }

    public DateTime TransactionDate { get; set; }

    public int? Accounts { get; set; }

    public decimal? BasicAmount { get; set; }

    public decimal? DiscountAmt { get; set; }

    public decimal? TotalAmount { get; set; }
}
