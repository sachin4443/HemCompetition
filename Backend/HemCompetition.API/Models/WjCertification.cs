using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjCertification
{
    public int CertificationId { get; set; }

    public int StudentId { get; set; }

    public int? CourseId { get; set; }

    public int? ExamId { get; set; }

    public int? MarksObtain { get; set; }

    public int? QuizId { get; set; }

    public string? YoutubeUrl { get; set; }

    public string? FileName { get; set; }

    public string? VideoDescription { get; set; }

    public bool? IsPaid { get; set; }

    public int? Amount { get; set; }

    public DateTime? ExamDate { get; set; }

    public DateTime? UploadDate { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? ModifyDate { get; set; }

    public bool? IsPass { get; set; }

    public int? ProjectMark { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public string? CertificationId1 { get; set; }

    public int? PlanningMarks { get; set; }

    public int? ResourcesUsageMarks { get; set; }

    public int? DifficultyLevelMarks { get; set; }

    public int? EngagementMarks { get; set; }

    public string? Remark { get; set; }
}
