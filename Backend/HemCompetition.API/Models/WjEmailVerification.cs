using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjEmailVerification
{
    public int EmailVerificationId { get; set; }

    public int StudentId { get; set; }

    public string Email { get; set; } = null!;

    public string? Guid { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool? Status { get; set; }
}
