namespace HemCompetition.API.Security;

/// <summary>Every permission the admin API checks. Roles are sets of these keys.</summary>
public static class Permissions
{
    public const string DashboardView = "dashboard.view";
    public const string StudentsView = "students.view";
    public const string StudentsManage = "students.manage";
    public const string GamesView = "games.view";
    public const string GamesManage = "games.manage";
    public const string QuizView = "quiz.view";
    public const string QuizManage = "quiz.manage";
    public const string QuizPublish = "quiz.publish";
    public const string CompetitionsView = "competitions.view";
    public const string CompetitionsManage = "competitions.manage";
    public const string AttemptsView = "attempts.view";
    public const string AttemptsManage = "attempts.manage";
    public const string ResultsView = "results.view";
    public const string ReportsExport = "reports.export";
    public const string ProctoringView = "proctoring.view";
    public const string ViolationsReview = "violations.review";
    public const string EvidenceView = "evidence.view";
    public const string EvidenceDelete = "evidence.delete";
    public const string BansView = "bans.view";
    public const string BansManage = "bans.manage";
    public const string AppealsView = "appeals.view";
    public const string AppealsReview = "appeals.review";
    public const string NotificationsSend = "notifications.send";
    public const string SessionsView = "sessions.view";
    public const string SessionsManage = "sessions.manage";
    public const string AdminsManage = "admins.manage";
    public const string RolesManage = "roles.manage";
    public const string SettingsManage = "settings.manage";
    public const string AuditView = "audit.view";
    public const string DiagnosticsView = "diagnostics.view";

    public static readonly IReadOnlyList<(string Key, string Group, string Label)> Catalog =
    [
        (DashboardView, "Dashboard", "View dashboard"),
        (StudentsView, "Students", "View students"),
        (StudentsManage, "Students", "Create, edit, import and reset students"),
        (GamesView, "Games", "View games and word bank"),
        (GamesManage, "Games", "Configure games and word bank"),
        (QuizView, "Mega Quiz", "View quizzes and question bank (with answers)"),
        (QuizManage, "Mega Quiz", "Create and edit quizzes and questions"),
        (QuizPublish, "Mega Quiz", "Publish and unpublish quizzes"),
        (CompetitionsView, "Competitions", "View competitions"),
        (CompetitionsManage, "Competitions", "Create, schedule and activate competitions"),
        (AttemptsView, "Attempts", "View game and quiz attempts"),
        (AttemptsManage, "Attempts", "Reset or flag attempts"),
        (ResultsView, "Results", "View results and leaderboard"),
        (ReportsExport, "Results", "Export reports"),
        (ProctoringView, "Proctoring", "View live monitoring and violations"),
        (ViolationsReview, "Proctoring", "Review violations and edit violation rules"),
        (EvidenceView, "Proctoring", "View evidence images and video"),
        (EvidenceDelete, "Proctoring", "Delete evidence"),
        (BansView, "Bans & appeals", "View bans"),
        (BansManage, "Bans & appeals", "Ban, suspend and restore students"),
        (AppealsView, "Bans & appeals", "View appeals"),
        (AppealsReview, "Bans & appeals", "Decide appeals"),
        (NotificationsSend, "Students", "Send notifications"),
        (SessionsView, "Sessions", "View login sessions"),
        (SessionsManage, "Sessions", "Terminate sessions"),
        (AdminsManage, "Administration", "Manage admin users"),
        (RolesManage, "Administration", "Manage roles and permissions"),
        (SettingsManage, "Administration", "Change security and system settings"),
        (AuditView, "Administration", "View audit log"),
        (DiagnosticsView, "Administration", "View API diagnostics"),
    ];

    public static readonly IReadOnlySet<string> All = Catalog.Select(c => c.Key).ToHashSet();

    public const string SuperAdminRole = "Super Admin";

    /// <summary>Roles created on first start. Super Admin implicitly holds every permission.</summary>
    public static readonly IReadOnlyList<(string Name, string Description, string[] Permissions)> DefaultRoles =
    [
        (SuperAdminRole, "Full access, including admin users and roles.", []),
        ("Admin", "Everything except admin users, roles and system settings.",
            All.Except([AdminsManage, RolesManage, SettingsManage]).ToArray()),
        ("Quiz Manager", "Maintains the question bank and quizzes.",
            [DashboardView, QuizView, QuizManage, QuizPublish, GamesView, ResultsView]),
        ("Competition Manager", "Schedules competitions and configures games.",
            [DashboardView, CompetitionsView, CompetitionsManage, GamesView, GamesManage, StudentsView, AttemptsView,
             ResultsView, ReportsExport, NotificationsSend, SessionsView]),
        ("Reviewer", "Reviews violations, evidence, bans and appeals.",
            [DashboardView, StudentsView, AttemptsView, ProctoringView, ViolationsReview, EvidenceView, BansView,
             BansManage, AppealsView, AppealsReview, SessionsView]),
        ("Read-only Auditor", "Can see everything except answer keys and evidence, and change nothing.",
            [DashboardView, StudentsView, GamesView, CompetitionsView, AttemptsView, ResultsView, ProctoringView,
             BansView, AppealsView, SessionsView, AuditView]),
    ];
}
