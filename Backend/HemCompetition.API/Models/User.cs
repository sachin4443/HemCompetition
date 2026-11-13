using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? UserCode { get; set; }

    public string? Title { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Gender { get; set; }

    public int? MaritalStatus { get; set; }

    public DateTime? Dob { get; set; }

    public string? MobileNo { get; set; }

    public string? PhoneNo { get; set; }

    public string? EmailId { get; set; }

    public string? AddressLine1 { get; set; }

    public string? AddressLine2 { get; set; }

    public string? AddressLine3 { get; set; }

    public int? City { get; set; }

    public string? Pincode { get; set; }

    public string? PhotoUrl { get; set; }

    public bool? IsActive { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string AspNetUserId { get; set; } = null!;

    public string? RegisterFrom { get; set; }

    public string? AccessToken { get; set; }

    public string? CityName { get; set; }

    public string? Password { get; set; }

    public bool? IsMobileVerified { get; set; }

    public bool? IsEmailVerified { get; set; }

    public string? LanguagePreference { get; set; }

    public bool? IsUserDeactivated { get; set; }

    public virtual ICollection<EventData2022> EventData2022s { get; set; } = new List<EventData2022>();

    public virtual ICollection<HfTblProject> HfTblProjects { get; set; } = new List<HfTblProject>();

    public virtual ICollection<HfTblTask> HfTblTaskAssignedToUsers { get; set; } = new List<HfTblTask>();

    public virtual ICollection<HfTblTask> HfTblTaskCreatedByUsers { get; set; } = new List<HfTblTask>();

    public virtual ICollection<Trainer> TrainerReferals { get; set; } = new List<Trainer>();

    public virtual ICollection<Trainer> TrainerReportToNavigations { get; set; } = new List<Trainer>();

    public virtual Trainer? TrainerTrainerNavigation { get; set; }

    public virtual ICollection<WjReceiptVoucher> WjReceiptVouchers { get; set; } = new List<WjReceiptVoucher>();

    public virtual ICollection<WjReferralCode> WjReferralCodes { get; set; } = new List<WjReferralCode>();

    public virtual ICollection<WjStudentFeedback> WjStudentFeedbacks { get; set; } = new List<WjStudentFeedback>();

    public virtual WjTeacher? WjTeacher { get; set; }
}
