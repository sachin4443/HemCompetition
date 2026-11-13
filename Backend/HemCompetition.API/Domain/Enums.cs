namespace HemCompetition.API.Domain;

public enum AttemptStatus { InProgress, Completed, Quit, Expired, Terminated }
public enum RoundStatus { Pending, Correct, Skipped }
public enum Difficulty { Easy, Medium, Hard }
public enum ViolationSeverity { Minor, Serious, Critical }
public enum ViolationAction { Recorded, Warning, Flagged, Terminated, Suspended }
public enum ReviewStatus { Open, Confirmed, Dismissed }
public enum BanType { Suspension, Ban }
public enum BanSource { Automatic, Admin }
public enum AppealStatus { Pending, Approved, Rejected }
public enum EvidenceKind { Image, Video }
public enum QuizSelectionMode { Fixed, Random }
