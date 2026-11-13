using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjScrambleTopic
{
    public int Id { get; set; }

    public string? ScrambleTopic { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }
}
