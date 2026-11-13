using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class ViewWjPracticalList
{
    public int SubmissionId { get; set; }

    public int? PracticalId { get; set; }

    public int? CourseId { get; set; }

    public string? Title { get; set; }

    public int? OrderNo { get; set; }

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

    public string? Status { get; set; }

    public int StudentId { get; set; }

    public string? Date { get; set; }

    public DateTime? SubmitDate { get; set; }

    public bool? IsTask { get; set; }

    public int? TeacherId { get; set; }

    public int Creativity { get; set; }

    public int Process { get; set; }

    public int Applicability { get; set; }

    public string? LevelName { get; set; }

    public string? Std { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public int? Coins { get; set; }

    public string? CityName { get; set; }

    public int InstituteId { get; set; }

    public string InstituteName { get; set; } = null!;

    public string? TeacherName { get; set; }

    public int? CommentCount { get; set; }

    public string? Code { get; set; }

    public string? StudentGroup { get; set; }

    public int? UserId { get; set; }

    public int? LanguageId { get; set; }

    public long? RowNum { get; set; }
}
