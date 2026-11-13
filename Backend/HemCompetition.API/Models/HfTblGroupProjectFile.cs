using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblGroupProjectFile
{
    public int FileId { get; set; }

    public int? MainCourseId { get; set; }

    public string? FileSource { get; set; }
}
