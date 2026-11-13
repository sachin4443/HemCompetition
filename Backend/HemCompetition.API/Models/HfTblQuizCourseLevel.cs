using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblQuizCourseLevel
{
    public int QuizId { get; set; }

    public int LevelId { get; set; }

    public int CourseId { get; set; }

    public virtual HfTblCourse Course { get; set; } = null!;

    public virtual HfTblLevel Level { get; set; } = null!;

    public virtual HfTblQuiz Quiz { get; set; } = null!;
}
