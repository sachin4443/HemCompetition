using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblCreativityActivitySubmission
{
    public int SubmissionId { get; set; }

    public int? InstituteId { get; set; }

    public string? ImageActivity { get; set; }

    public string? VideoActivity { get; set; }

    public string? ShortDescription { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? MainCourseId { get; set; }

    public string? GroupName { get; set; }

    public int? StudentId { get; set; }

    public bool? IsAllocated { get; set; }

    public int? IsAllocatedBy { get; set; }
}
