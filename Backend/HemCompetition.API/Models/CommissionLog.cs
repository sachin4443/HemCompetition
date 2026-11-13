using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class CommissionLog
{
    public int CommissionLogId { get; set; }

    public int? UserId { get; set; }

    public decimal? RegistrationAmount { get; set; }

    public decimal? CommissionAmount { get; set; }

    public decimal? FreeStudentCommssion { get; set; }

    public decimal? CommissionPerAffiliate { get; set; }

    public decimal? CreditCommission { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    public int Status { get; set; }
}
