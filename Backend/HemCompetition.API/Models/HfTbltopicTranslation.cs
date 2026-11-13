using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTbltopicTranslation
{
    public int TranslationId { get; set; }

    public int TopicId { get; set; }

    public int LanguageId { get; set; }

    public string? VisibleTopicName { get; set; }

    public string? VisibleDescription { get; set; }

    public string? VisibleTrainerVideo { get; set; }

    public string? VisibleTrainerResource { get; set; }

    public string? VisibleTrainerExercise { get; set; }
}
