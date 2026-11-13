using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class CustomerTypeLookup
{
    public int CustomerTypeId { get; set; }

    public string CustomerType { get; set; } = null!;

    public string? Description { get; set; }

    public int? OrderBy { get; set; }

    public bool IsActive { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }
}
