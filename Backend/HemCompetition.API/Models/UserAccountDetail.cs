using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class UserAccountDetail
{
    public int AccountId { get; set; }

    public int UserId { get; set; }

    public string AdharcardPath { get; set; } = null!;

    public string PancardPath { get; set; } = null!;

    public string? BankName { get; set; }

    public string? AccountNumber { get; set; }

    public string? IfscCode { get; set; }

    public bool? Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? ChequePath { get; set; }
}
