using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class ViewQuestion
{
    public int QuestionId { get; set; }

    public string? QuestionName { get; set; }

    public int? Marks { get; set; }

    public string? Type { get; set; }

    public bool Status { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? ComplexityLevel { get; set; }

    public string? AnswerDescription { get; set; }

    public int? CourseId { get; set; }

    public int? TopicId { get; set; }

    public string? CourseName { get; set; }

    public int? Author { get; set; }

    public string? FeaturedImage { get; set; }

    public int? NoOfSessions { get; set; }

    public string? CourseDescription { get; set; }

    public string? TopicName { get; set; }

    public string? TopicDescription { get; set; }

    public string? SubTopicName { get; set; }

    public int? LanguageId { get; set; }
}
