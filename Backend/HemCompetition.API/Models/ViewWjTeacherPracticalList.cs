using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class ViewWjTeacherPracticalList
{
    public int SubmissionId { get; set; }

    public int? PracticalId { get; set; }

    public int? CourseId { get; set; }

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

    public string? CourseName { get; set; }

    public string? ProjectName { get; set; }

    public string? FileLink { get; set; }

    public string? Status { get; set; }

    public string? Date { get; set; }

    public DateTime? SubmitDate { get; set; }

    public string? LevelName { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public int? Coin { get; set; }

    public long? Rank { get; set; }

    public int? CommentCount { get; set; }

    public string? Code { get; set; }

    public string? StudentGroup { get; set; }
}
