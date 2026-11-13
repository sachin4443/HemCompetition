using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjEventType
{
    public int EventTypeId { get; set; }

    public string? Name { get; set; }

    public int? DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public string? Message { get; set; }

    public DateTime? CreatedOn { get; set; }

    public virtual ICollection<WjEvent> WjEvents { get; set; } = new List<WjEvent>();
}
