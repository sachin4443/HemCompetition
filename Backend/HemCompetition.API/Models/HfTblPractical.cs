using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblPractical
{
    public int PracticalId { get; set; }

    public int CourseId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public int? OrderNo { get; set; }

    public int? Coins { get; set; }

    public string? FileType { get; set; }

    public string? VideoLink { get; set; }

    public string? Thumbnail { get; set; }

    public int? PracticalLevel { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? Code { get; set; }

    public string? StudentGroup { get; set; }

    public string? Resources { get; set; }

    public int? Gems { get; set; }

    public int? PracticalTopicId { get; set; }

    public int? PracticalSubtopicId { get; set; }

    public string? PracticalStandard { get; set; }

    public bool? IsTask { get; set; }

    public int? LanguageId { get; set; }

    public int? ScrambleTopicId { get; set; }

    public string? Type { get; set; }

    public virtual HfTblCourse Course { get; set; } = null!;
}
