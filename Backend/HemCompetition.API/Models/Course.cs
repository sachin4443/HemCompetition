using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class Course
{
    public int CourseId { get; set; }

    public string CourseName { get; set; } = null!;

    public int NoOfSessions { get; set; }

    public string? Description { get; set; }

    public decimal? Fees { get; set; }

    public bool IsActive { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? FeaturedImage { get; set; }

    public int? Author { get; set; }

    public bool? IsPublished { get; set; }

    public DateTime? PublishDate { get; set; }

    public DateTime? ExpectedDeliveryDate { get; set; }

    public string? ApprovalRemark { get; set; }

    public string? ApprovalStatus { get; set; }

    public DateTime? ApprovedOn { get; set; }

    public int? ApprovedBy { get; set; }

    public string? TrainerResource { get; set; }

    public string? StudentResourse { get; set; }

    public string? TrainerExercise { get; set; }

    public string? StudentExercise { get; set; }

    public int? Manager { get; set; }

    public string CourseAllocatedTo { get; set; } = null!;

    public int? CourseParentId { get; set; }

    public int? FromAge { get; set; }

    public int? ToAge { get; set; }

    public string? MainCategory { get; set; }
}
