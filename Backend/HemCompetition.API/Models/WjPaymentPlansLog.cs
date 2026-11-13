using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjPaymentPlansLog
{
    public int PaymentId { get; set; }

    public string PaymentType { get; set; } = null!;

    public decimal Amount { get; set; }

    public int Credit { get; set; }

    public int Coins { get; set; }

    public DateTime Validity { get; set; }

    public DateTime ExpiredOn { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime? UpadtedOn { get; set; }

    public bool? Status { get; set; }
}
