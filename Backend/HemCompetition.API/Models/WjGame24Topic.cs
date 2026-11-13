using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjGame24Topic
{
    public int GameTopicId { get; set; }

    public string? GameTopicName { get; set; }

    public bool? GameTopicIsActive { get; set; }

    public string? GameDescription { get; set; }

    public virtual ICollection<WjGame24> WjGame24s { get; set; } = new List<WjGame24>();
}
