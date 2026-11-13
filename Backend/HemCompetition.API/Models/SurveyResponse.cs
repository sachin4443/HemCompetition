using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class SurveyResponse
{
    public int? QuestionNumber { get; set; }

    public string? QuestionText { get; set; }

    public string? SelectedOption { get; set; }

    public int? CourseId { get; set; }

    public int? TopicId { get; set; }

    public string? TopicName { get; set; }

    public int? UserId { get; set; }

    public int? Point { get; set; }

    public DateTime? SubmitTime { get; set; }
}
