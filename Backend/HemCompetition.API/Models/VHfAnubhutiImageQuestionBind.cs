using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class VHfAnubhutiImageQuestionBind
{
    public int AnbId { get; set; }

    public int? CourseId { get; set; }

    public string? AnbQuestion { get; set; }

    public byte[]? SmileImage { get; set; }

    public byte[]? SadImage { get; set; }

    public string? AnbType { get; set; }
}
