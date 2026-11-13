using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class MainCourse
{
    public int MainCourseId { get; set; }

    public string MainCourseName { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? MainCourseTemplateUrl { get; set; }

    public int? MainCourseOrderNo { get; set; }
}
