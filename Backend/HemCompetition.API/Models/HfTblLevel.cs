using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblLevel
{
    public int LevelId { get; set; }

    public string? HfTblLevels { get; set; }

    public virtual ICollection<HfTblQuizCourseLevel> HfTblQuizCourseLevels { get; set; } = new List<HfTblQuizCourseLevel>();
}
