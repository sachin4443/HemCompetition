using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblPracticalTranslation
{
    public int TranslationId { get; set; }

    public int PracticalId { get; set; }

    public int LanguageId { get; set; }

    public string? VisibleTitle { get; set; }

    public string? VisibleDescription { get; set; }

    public string? VisibleVideoLink { get; set; }

    public string? VisibleThumbnail { get; set; }

    public DateTime? CreatedDate { get; set; }
}
