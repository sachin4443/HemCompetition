using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfViewTeacherList
{
    public int TeacherId { get; set; }

    public string? Standard { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Gender { get; set; }

    public string? MobileNo { get; set; }

    public string? EmailId { get; set; }

    public string? Pincode { get; set; }

    public string? CityName { get; set; }

    public int? InstituteId { get; set; }

    public string? InstituteName { get; set; }

    public bool? IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserName { get; set; }

    public string? Password { get; set; }
}
