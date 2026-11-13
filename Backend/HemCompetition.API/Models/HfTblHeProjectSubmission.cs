using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblHeProjectSubmission
{
    public int SubmissionId { get; set; }

    public int? StudentsId { get; set; }

    public string? TeamMembersList { get; set; }

    public string? GroupFilePath { get; set; }

    public string? VideoActivity { get; set; }

    public string? ShortDescription { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CourseId { get; set; }

    public int? CreatedBy { get; set; }

    public string? ProjectName { get; set; }
}
