using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjValueCertification
{
    public int Id { get; set; }

    public string? CertificationNo { get; set; }

    public string? Type { get; set; }

    public int? UserId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Standard { get; set; }

    public string? Division { get; set; }

    public string? EmailId { get; set; }

    public string? MobileNo { get; set; }

    public int? InstituteId { get; set; }

    public string? InstituteName { get; set; }

    public string? Lavel { get; set; }

    public int? Points { get; set; }

    public int? Rank { get; set; }

    public int? QuizId { get; set; }

    public int? CourseId { get; set; }

    public string? CourseName { get; set; }

    public DateTime? ExpiredOn { get; set; }

    public DateTime? GeneratedOn { get; set; }

    public DateTime? IssuedOn { get; set; }

    public bool? Status { get; set; }

    public bool? IsCompleted { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsAdminStatus { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public int? ModifiedBy { get; set; }
}
