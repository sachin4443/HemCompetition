using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class ViewWjSyllabus
{
    public int SyllabusId { get; set; }

    public string? CompetitionYear { get; set; }

    public string? Standard { get; set; }

    public int? MainCourseId { get; set; }

    public int? CourseId { get; set; }

    public int? QuizId { get; set; }

    public int? InstituteId { get; set; }

    public string? MainCourseName { get; set; }

    public string? MainCourseDescription { get; set; }

    public string? CourseName { get; set; }

    public int? MainCourseOrderNo { get; set; }

    public string? QuizName { get; set; }

    public string? CategoryName { get; set; }

    public string? SubCategoryName { get; set; }

    public int? OrderNo { get; set; }
}
