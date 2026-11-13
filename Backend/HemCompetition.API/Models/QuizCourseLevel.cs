using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class QuizCourseLevel
{
    public int QuizId { get; set; }

    public int LevelId { get; set; }

    public int CourseId { get; set; }
}
