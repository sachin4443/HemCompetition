using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblQuiz
{
    public int QuizId { get; set; }

    public string Name { get; set; } = null!;

    public DateTime? StartDateTime { get; set; }

    public DateTime? EndDateTime { get; set; }

    public int? Duration { get; set; }

    public string? Type { get; set; }

    public int? PassMarks { get; set; }

    public int? TotalMarks { get; set; }

    public int? Attempts { get; set; }

    public bool? IsActive { get; set; }

    public bool? WhizJuniorSyllabus { get; set; }

    public string? CourseAllocatedTo { get; set; }

    public string? Mcqlevel { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual ICollection<HfTblQuizCourseLevel> HfTblQuizCourseLevels { get; set; } = new List<HfTblQuizCourseLevel>();
}
