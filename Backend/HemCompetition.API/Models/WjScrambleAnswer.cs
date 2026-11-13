using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjScrambleAnswer
{
    public int AnsId { get; set; }

    public int? QuestionId { get; set; }

    public int? ScrambleTopicId { get; set; }

    public string? Answer { get; set; }

    public int? UserId { get; set; }

    public DateTime? CreatedDate { get; set; }
}
