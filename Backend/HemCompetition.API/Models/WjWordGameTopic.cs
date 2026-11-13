using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjWordGameTopic
{
    public int WgTopicId { get; set; }

    public string? WgTopicName { get; set; }

    public string? Description { get; set; }

    public bool? IsActive { get; set; }
}
