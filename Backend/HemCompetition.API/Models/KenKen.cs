using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class KenKen
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? KenkenList { get; set; }

    public bool? Status { get; set; }

    public bool? IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? Createdon { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }
}
