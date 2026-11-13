using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblCreativityActivitesSubmission2026
{
    public int SubmissionId { get; set; }

    public int? InstituteId { get; set; }

    public string? TeamMembersList { get; set; }

    public string? ImageActivity { get; set; }

    public string? VideoActivity { get; set; }

    public string? ShortDescription { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? MainCourseId { get; set; }

    public string? ActivityType { get; set; }

    public bool? IsBestProject { get; set; }
}
