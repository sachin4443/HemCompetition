using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblParentChildAssociation
{
    public int? ParentId { get; set; }

    public int? ChildId { get; set; }

    public DateTime? MappedDate { get; set; }
}
