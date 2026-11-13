using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfRelManageGroupListDeletedatum
{
    public int Mgslid { get; set; }

    public string? GroupName { get; set; }

    public int? StudentId { get; set; }

    public DateTime? EntryDate { get; set; }

    public bool? IsAllocated { get; set; }

    public int? IsAllocatedBy { get; set; }

    public string? ActivityType { get; set; }

    public int? MainCourseId { get; set; }

    public int? Deletedby { get; set; }

    public DateTime? DeletedDate { get; set; }
}
