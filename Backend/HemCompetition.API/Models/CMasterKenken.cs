using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class CMasterKenken
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? HeaderIds { get; set; }

    public string? IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }
}
