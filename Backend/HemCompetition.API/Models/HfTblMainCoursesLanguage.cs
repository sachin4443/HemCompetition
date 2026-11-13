using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblMainCoursesLanguage
{
    public int Id { get; set; }

    public int MainCourseId { get; set; }

    public string? Language { get; set; }

    public string? LanguageImagePath { get; set; }

    public string? LanguageDescription { get; set; }

    public bool IsActive { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }
}
