using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjCourseSetup
{
    public int Id { get; set; }

    public int CourseId { get; set; }

    public string? CourseName { get; set; }

    public int? Type { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? VideoUrl { get; set; }

    public string? WebinarLink { get; set; }

    public string? ImageUrl { get; set; }

    public string? Pdfurl { get; set; }

    public string? PdfimageUrl { get; set; }

    public int? OrderNo { get; set; }

    public bool? IsActive { get; set; }

    public bool? Status { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public int? ModifiedBy { get; set; }
}
