using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblSurveyDataTranslation
{
    public int TranslationId { get; set; }

    public int? ParentId { get; set; }

    public int? LanguageId { get; set; }

    public int? CourseId { get; set; }

    public int? TopicId { get; set; }

    public string? TopicName { get; set; }

    public int? SubTopicId { get; set; }

    public string? Question { get; set; }

    public string? Option1 { get; set; }

    public string? Option2 { get; set; }

    public string? Option3 { get; set; }

    public string? Option4 { get; set; }

    public string? Option5 { get; set; }
}
