using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfVisitProcessRunning
{
    public int ProcessId { get; set; }

    public DateTime? EntryDate { get; set; }

    public int? Kpid { get; set; }

    public string? KpidPartnerUploadSignStatus { get; set; }

    public DateTime? SchoolOrientationVisitDateTime { get; set; }

    public string? SchoolOrientationOnVisitLocationName { get; set; }

    public string? SchoolOrientationVisitImageStatus { get; set; }

    public string? SchoolOrientationVisitStatusCompleted { get; set; }

    public string? SchoolOrientationRemarks { get; set; }

    public DateTime? TeacherOrientationVisitDateTime { get; set; }

    public string? TeacherOrientationOnVistiLocationName { get; set; }

    public string? TeacherOrientationVisitImageStatus { get; set; }

    public string? TeacherOrientationVisitStatusCompleted { get; set; }

    public string? TeacherOrientationRemarks { get; set; }

    public DateTime? StudentOrientationVisitDateTime { get; set; }

    public string? StudnetOrientationOnVisitLocationName { get; set; }

    public string? StudentOrientationVisitImageStatus { get; set; }

    public DateTime? StudentOrientationVisitStatusCompleted { get; set; }

    public string? StudentOrientationRemarks { get; set; }

    public string? IsSchoolFeedBack { get; set; }

    public string? FullyProcessStatus { get; set; }

    public int? AffileatedUserId { get; set; }
}
