using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjInstitute
{
    public int InstituteId { get; set; }

    public string InstituteType { get; set; } = null!;

    public string? Name { get; set; }

    public string? BannerImageUrl { get; set; }

    public string? ProfileUrl { get; set; }

    public bool? IsActive { get; set; }

    public string? ReferralCode { get; set; }

    public bool IsVerify { get; set; }

    public string? Profession { get; set; }

    public string? SocialOrgName { get; set; }

    public string? TypeOfOrganisation { get; set; }

    public byte[]? CertificateImage { get; set; }

    public virtual ICollection<HfTblAcademicTerm> HfTblAcademicTerms { get; set; } = new List<HfTblAcademicTerm>();

    public virtual ICollection<HfTblExamConfiguration> HfTblExamConfigurations { get; set; } = new List<HfTblExamConfiguration>();

    public virtual ICollection<HfTblInstituteSubject> HfTblInstituteSubjects { get; set; } = new List<HfTblInstituteSubject>();

    public virtual ICollection<HfTblStudentMark> HfTblStudentMarks { get; set; } = new List<HfTblStudentMark>();

    public virtual ICollection<WjMainCertificate> WjMainCertificates { get; set; } = new List<WjMainCertificate>();

    public virtual ICollection<WjStudentAssociate> WjStudentAssociates { get; set; } = new List<WjStudentAssociate>();

    public virtual ICollection<WjTeacherAssociate> WjTeacherAssociates { get; set; } = new List<WjTeacherAssociate>();
}
