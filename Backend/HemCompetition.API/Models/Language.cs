using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class Language
{
    public int LanguageId { get; set; }

    public string? LanguageName { get; set; }

    public DateTime CreatedDate { get; set; }
}
