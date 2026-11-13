using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjScrambleQuestion
{
    public int Id { get; set; }

    public int TopicId { get; set; }

    public string? ScrambleType { get; set; }

    public string? Question { get; set; }

    public string? Answer { get; set; }

    public string? JumbledWord { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public int? CourseId { get; set; }
}
