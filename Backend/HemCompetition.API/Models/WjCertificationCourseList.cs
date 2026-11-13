using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjCertificationCourseList
{
    public int Id { get; set; }

    public string? CourseName { get; set; }

    public int? QuizId { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string Role { get; set; } = null!;
}
