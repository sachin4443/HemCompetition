using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblParent
{
    public int? ParentId { get; set; }

    public string? Occupation { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }
}
