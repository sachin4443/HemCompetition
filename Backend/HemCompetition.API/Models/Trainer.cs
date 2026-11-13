using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class Trainer
{
    public int TrainerId { get; set; }

    public string RegistrationMode { get; set; } = null!;

    public int? ReferalId { get; set; }

    public int? ReportTo { get; set; }

    public DateTime? DateOfJoining { get; set; }

    public DateTime? DateOfResignation { get; set; }

    public int? AppointedBy { get; set; }

    public string? AlternateContactPerson { get; set; }

    public int? ContactPersonRelation { get; set; }

    public string? ContactPersonContactNo { get; set; }

    public string? PermanentAddressLine1 { get; set; }

    public string? PermanentAddressLine2 { get; set; }

    public string? PermanentAddressLine3 { get; set; }

    public int? PermanentCity { get; set; }

    public string? PermanentPincode { get; set; }

    public decimal? ResidentialLatitude { get; set; }

    public decimal? ResidentialLongitude { get; set; }

    public string? PhotoIdentityProofUrl { get; set; }

    public bool? IsIdentityProofVerified { get; set; }

    public bool? InterestedInLearning { get; set; }

    public bool? IsApproved { get; set; }

    public int? ApprovedBy { get; set; }

    public DateTime? ApprovedDate { get; set; }

    public string? ApprovalRemark { get; set; }

    public DateTime? PreferedCalltimeFrom { get; set; }

    public DateTime? PreferedCalltimeTo { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? AddressIdentityProofUrl { get; set; }

    public virtual User? Referal { get; set; }

    public virtual User? ReportToNavigation { get; set; }

    public virtual User TrainerNavigation { get; set; } = null!;
}
