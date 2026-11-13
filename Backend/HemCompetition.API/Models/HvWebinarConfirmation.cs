using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HvWebinarConfirmation
{
    public int WebinarId { get; set; }

    public int? TeacherId { get; set; }

    public string? WebinarTopicSelected { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
