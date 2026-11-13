namespace HemCompetition.API.Domain;

public class ProctoringSession
{
    public Guid Id { get; set; }
    public long StudentId { get; set; }
    public Guid? CompetitionId { get; set; }
    public Guid? AuthSessionId { get; set; }
    public string? ConsentVersion { get; set; }
    public bool EvidenceConsent { get; set; }
    public bool VideoConsent { get; set; }
    public DateTime StartedAtUtc { get; set; }
    public DateTime? EndedAtUtc { get; set; }
    public DateTime? LastSeenAtUtc { get; set; }
    public string? LastCameraStatus { get; set; }
    public int? LastPersonCount { get; set; }
    public string? UserAgent { get; set; }
    public string? IpAddress { get; set; }
}

/// <summary>Configurable policy for one kind of violation.</summary>
public class ViolationRule
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ViolationSeverity Severity { get; set; }
    public bool IsEnabled { get; set; } = true;
    public bool CaptureImage { get; set; }
    public bool CaptureVideo { get; set; }
    /// <summary>Occurrences (per student, per competition) after which the active attempt is flagged for review. 0 = never.</summary>
    public int FlagAfter { get; set; }
    /// <summary>Occurrences after which the active attempt is terminated. 0 = never.</summary>
    public int TerminateAfter { get; set; }
    /// <summary>Occurrences after which the student is suspended pending review. 0 = never.</summary>
    public int SuspendAfter { get; set; }
    /// <summary>Length of an automatic suspension. Null = until an administrator reviews it.</summary>
    public int? SuspendMinutes { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}

public class ViolationEvent
{
    public Guid Id { get; set; }
    public long StudentId { get; set; }
    public Guid? CompetitionId { get; set; }
    public Guid? ProctoringSessionId { get; set; }
    public string RuleCode { get; set; } = string.Empty;
    public ViolationSeverity Severity { get; set; }
    public string? Message { get; set; }
    public string? Details { get; set; }
    public Guid? GameAttemptId { get; set; }
    public Guid? QuizAttemptId { get; set; }
    public int OccurrenceNo { get; set; }
    public ViolationAction ActionTaken { get; set; }
    public DateTime OccurredAtUtc { get; set; }
    public ReviewStatus ReviewStatus { get; set; }
    public Guid? ReviewedByAdminId { get; set; }
    public DateTime? ReviewedAtUtc { get; set; }
    public string? ReviewNote { get; set; }
    public List<EvidenceFile> Evidence { get; set; } = new();
}

/// <summary>Metadata of a captured image or clip. The bytes live in file storage under <see cref="StorageKey"/>.</summary>
public class EvidenceFile
{
    public Guid Id { get; set; }
    public Guid ViolationEventId { get; set; }
    public long StudentId { get; set; }
    public EvidenceKind Kind { get; set; }
    public string StorageKey { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string Sha256 { get; set; } = string.Empty;
    public DateTime CapturedAtUtc { get; set; }
    public DateTime RetainUntilUtc { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public string? DeletedBy { get; set; }
    public ViolationEvent ViolationEvent { get; set; } = null!;
}

public class StudentBan
{
    public Guid Id { get; set; }
    public long StudentId { get; set; }
    public Guid? CompetitionId { get; set; }
    public BanType Type { get; set; }
    public BanSource Source { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? RuleCode { get; set; }
    public Guid? ViolationEventId { get; set; }
    public DateTime StartsAtUtc { get; set; }
    /// <summary>Null = no automatic end.</summary>
    public DateTime? EndsAtUtc { get; set; }
    public Guid? CreatedByAdminId { get; set; }
    public DateTime? LiftedAtUtc { get; set; }
    public Guid? LiftedByAdminId { get; set; }
    public string? LiftReason { get; set; }
    public List<Appeal> Appeals { get; set; } = new();

    public bool IsActiveAt(DateTime nowUtc) =>
        LiftedAtUtc == null && StartsAtUtc <= nowUtc && (EndsAtUtc == null || EndsAtUtc > nowUtc);
}

public class Appeal
{
    public Guid Id { get; set; }
    public Guid BanId { get; set; }
    public long StudentId { get; set; }
    public string Message { get; set; } = string.Empty;
    public AppealStatus Status { get; set; }
    public DateTime SubmittedAtUtc { get; set; }
    public DateTime? DecidedAtUtc { get; set; }
    public Guid? DecidedByAdminId { get; set; }
    /// <summary>Shown to the student.</summary>
    public string? DecisionReason { get; set; }
    public StudentBan Ban { get; set; } = null!;
    public List<AppealNote> Notes { get; set; } = new();
}

/// <summary>Internal reviewer note or recorded decision step. Never shown to the student.</summary>
public class AppealNote
{
    public long Id { get; set; }
    public Guid AppealId { get; set; }
    public Guid AdminId { get; set; }
    public string AdminName { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public Appeal Appeal { get; set; } = null!;
}
