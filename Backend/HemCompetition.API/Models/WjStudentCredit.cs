using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjStudentCredit
{
    public int CreditId { get; set; }

    public int StudentId { get; set; }

    public string? CreditFor { get; set; }

    public int? ReferenceId { get; set; }

    public int? Credit { get; set; }

    public DateTime? CreatedOn { get; set; }

    public DateTime? ModifyOn { get; set; }
}
