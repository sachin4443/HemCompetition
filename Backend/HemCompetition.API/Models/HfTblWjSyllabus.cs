using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblWjSyllabus
{
    public int SyllabusId { get; set; }

    public string? CompetitionYear { get; set; }

    public string? Standard { get; set; }

    public int? CourseId { get; set; }

    public int? QuizId { get; set; }

    public int? InstituteId { get; set; }

    public int? CategoryId { get; set; }

    public int? SubCategoryId { get; set; }

    public int? OrderNo { get; set; }

    public int? MainCourseId { get; set; }
}
