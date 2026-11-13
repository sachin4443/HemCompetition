using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblHegroupInvitation
{
    public int Id { get; set; }

    public int? SubmissionId { get; set; }

    public int? AcceptedStudentId { get; set; }

    public int? InvitedBy { get; set; }

    public DateTime? AcceptDate { get; set; }
}
