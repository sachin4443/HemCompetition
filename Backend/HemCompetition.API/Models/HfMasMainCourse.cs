using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfMasMainCourse
{
    public int MainCourseId { get; set; }

    public string MainCourseName { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public int EntBy { get; set; }

    public DateTime EntDate { get; set; }

    public DateTime? UpdDate { get; set; }

    public int? UpdBy { get; set; }

    public string? CertificateTemplateUrl { get; set; }

    public int? MainCourseOrderNo { get; set; }

    public int? RecStatus { get; set; }
}
