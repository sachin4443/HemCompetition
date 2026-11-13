using System.Text.RegularExpressions;
using HemCompetition.API.Domain;
using Microsoft.EntityFrameworkCore;

namespace HemCompetition.API.Data;

public partial class AppDbContext
{
    public DbSet<AdminRole> AdminRoles => Set<AdminRole>();
    public DbSet<AdminRolePermission> AdminRolePermissions => Set<AdminRolePermission>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationRead> NotificationReads => Set<NotificationRead>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();
    public DbSet<GameCategory> GameCategories => Set<GameCategory>();
    public DbSet<Game> Games => Set<Game>();
    public DbSet<WordBankEntry> WordBank => Set<WordBankEntry>();
    public DbSet<GameAttempt> GameAttempts => Set<GameAttempt>();
    public DbSet<GameAttemptRound> GameAttemptRounds => Set<GameAttemptRound>();
    public DbSet<QuizCategory> QuizCategories => Set<QuizCategory>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<QuestionOption> QuestionOptions => Set<QuestionOption>();
    public DbSet<Quiz> Quizzes => Set<Quiz>();
    public DbSet<QuizQuestion> QuizQuestions => Set<QuizQuestion>();
    public DbSet<QuizAttempt> QuizAttempts => Set<QuizAttempt>();
    public DbSet<QuizAttemptQuestion> QuizAttemptQuestions => Set<QuizAttemptQuestion>();
    public DbSet<ProctoringSession> ProctoringSessions => Set<ProctoringSession>();
    public DbSet<ViolationRule> ViolationRules => Set<ViolationRule>();
    public DbSet<ViolationEvent> ViolationEvents => Set<ViolationEvent>();
    public DbSet<EvidenceFile> EvidenceFiles => Set<EvidenceFile>();
    public DbSet<StudentBan> StudentBans => Set<StudentBan>();
    public DbSet<Appeal> Appeals => Set<Appeal>();
    public DbSet<AppealNote> AppealNotes => Set<AppealNote>();

    private static void ConfigurePlatform(ModelBuilder b)
    {
        // Students are referenced by auth_users.user_id (the id every existing table already uses).
        void Student<T>(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<T> e, string fk, DeleteBehavior del = DeleteBehavior.Restrict)
            where T : class =>
            e.HasOne<AuthUser>().WithMany().HasForeignKey(fk).HasPrincipalKey(u => u.UserId).OnDelete(del);

        b.Entity<AdminUser>(e =>
        {
            e.Property(x => x.Email).HasMaxLength(256);
            e.HasOne(x => x.Role).WithMany().HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict);
        });

        b.Entity<CompetitionSetting>(e =>
        {
            e.Property(x => x.Description).HasMaxLength(2000);
            e.Property(x => x.ProctoringRequired).HasDefaultValue(true);
        });

        b.Entity<AdminRole>(e =>
        {
            e.ToTable("admin_roles");
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Description).HasMaxLength(500);
            e.HasIndex(x => x.Name).IsUnique();
        });
        b.Entity<AdminRolePermission>(e =>
        {
            e.ToTable("admin_role_permissions");
            e.HasKey(x => new { x.RoleId, x.Permission });
            e.Property(x => x.Permission).HasMaxLength(100);
            e.HasOne(x => x.Role).WithMany(r => r.Permissions).HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Cascade);
        });
        b.Entity<RefreshToken>(e =>
        {
            e.ToTable("refresh_tokens");
            e.Property(x => x.TokenHash).HasMaxLength(64).IsRequired();
            e.HasIndex(x => x.TokenHash).IsUnique();
            e.HasIndex(x => x.FamilyId);
            e.HasIndex(x => x.ExpiresAtUtc);
            Student(e, nameof(RefreshToken.StudentId), DeleteBehavior.Cascade);
            e.HasOne<AdminUser>().WithMany().HasForeignKey(x => x.AdminId).OnDelete(DeleteBehavior.Cascade);
            e.ToTable(t => t.HasCheckConstraint("ck_refresh_tokens_subject", "(student_id IS NOT NULL) <> (admin_id IS NOT NULL)"));
        });
        b.Entity<Notification>(e =>
        {
            e.ToTable("notifications");
            e.Property(x => x.Title).HasMaxLength(200).IsRequired();
            e.Property(x => x.Body).HasMaxLength(4000).IsRequired();
            e.Property(x => x.Category).HasMaxLength(30);
            e.HasIndex(x => new { x.StudentId, x.CreatedAtUtc });
            Student(e, nameof(Notification.StudentId), DeleteBehavior.Cascade);
        });
        b.Entity<NotificationRead>(e =>
        {
            e.ToTable("notification_reads");
            e.HasKey(x => new { x.NotificationId, x.StudentId });
            e.HasOne<Notification>().WithMany().HasForeignKey(x => x.NotificationId).OnDelete(DeleteBehavior.Cascade);
            Student(e, nameof(NotificationRead.StudentId), DeleteBehavior.Cascade);
        });
        b.Entity<AuditLog>(e =>
        {
            e.ToTable("audit_logs");
            e.Property(x => x.ActorType).HasMaxLength(20);
            e.Property(x => x.ActorId).HasMaxLength(64);
            e.Property(x => x.ActorName).HasMaxLength(256);
            e.Property(x => x.Action).HasMaxLength(100).IsRequired();
            e.Property(x => x.EntityType).HasMaxLength(64);
            e.Property(x => x.EntityId).HasMaxLength(64);
            e.Property(x => x.Details).HasColumnType("jsonb");
            e.Property(x => x.IpAddress).HasMaxLength(64);
            e.HasIndex(x => x.CreatedAtUtc);
            e.HasIndex(x => new { x.EntityType, x.EntityId });
        });
        b.Entity<SystemSetting>(e =>
        {
            e.ToTable("system_settings");
            e.HasKey(x => x.Key);
            e.Property(x => x.Key).HasMaxLength(100);
            e.Property(x => x.UpdatedBy).HasMaxLength(256);
        });

        b.Entity<GameCategory>(e =>
        {
            e.ToTable("game_categories");
            e.Property(x => x.Key).HasMaxLength(50).IsRequired();
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Description).HasMaxLength(500);
            e.HasIndex(x => x.Key).IsUnique();
        });
        b.Entity<Game>(e =>
        {
            e.ToTable("games", t =>
            {
                t.HasCheckConstraint("ck_games_rounds", "rounds BETWEEN 1 AND 50");
                t.HasCheckConstraint("ck_games_points", "points_per_round >= 0");
            });
            e.Property(x => x.Key).HasMaxLength(50).IsRequired();
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Description).HasMaxLength(1000);
            e.Property(x => x.EngineKey).HasMaxLength(50).IsRequired();
            e.Property(x => x.PointsPerRound).HasPrecision(8, 2);
            e.Property(x => x.Config).HasColumnType("jsonb");
            e.HasIndex(x => x.Key).IsUnique();
            e.HasOne(x => x.Category).WithMany(c => c.Games).HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
        });
        b.Entity<WordBankEntry>(e =>
        {
            e.ToTable("word_bank");
            e.Property(x => x.Word).HasMaxLength(40).IsRequired();
            e.Property(x => x.Clue).HasMaxLength(500);
            e.Property(x => x.Topic).HasMaxLength(100);
            e.HasIndex(x => x.Word).IsUnique();
        });
        b.Entity<GameAttempt>(e =>
        {
            e.ToTable("game_attempts");
            e.Property(x => x.ConfigSnapshot).HasColumnType("jsonb");
            e.Property(x => x.Score).HasPrecision(10, 2);
            e.Property(x => x.MaxScore).HasPrecision(10, 2);
            // One attempt per student, per game, per competition - also what stops duplicate starts.
            e.HasIndex(x => new { x.CompetitionId, x.StudentId, x.GameId }).IsUnique();
            e.HasIndex(x => x.StudentId);
            e.HasIndex(x => new { x.GameId, x.Status });
            e.HasOne(x => x.Game).WithMany().HasForeignKey(x => x.GameId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Competition).WithMany().HasForeignKey(x => x.CompetitionId).OnDelete(DeleteBehavior.Restrict);
            Student(e, nameof(GameAttempt.StudentId));
        });
        b.Entity<GameAttemptRound>(e =>
        {
            e.ToTable("game_attempt_rounds");
            e.Property(x => x.Puzzle).HasColumnType("jsonb");
            e.Property(x => x.Solution).HasColumnType("jsonb");
            e.Property(x => x.Submission).HasColumnType("jsonb");
            e.Property(x => x.Score).HasPrecision(8, 2);
            e.HasIndex(x => new { x.AttemptId, x.RoundNo }).IsUnique();
            e.HasOne(x => x.Attempt).WithMany(a => a.Rounds).HasForeignKey(x => x.AttemptId).OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<QuizCategory>(e =>
        {
            e.ToTable("quiz_categories");
            e.Property(x => x.Name).HasMaxLength(150).IsRequired();
            e.Property(x => x.Description).HasMaxLength(1000);
            e.HasIndex(x => x.Name).IsUnique();
        });
        b.Entity<Question>(e =>
        {
            e.ToTable("questions", t => t.HasCheckConstraint("ck_questions_marks", "marks > 0"));
            e.Property(x => x.Text).HasMaxLength(4000).IsRequired();
            e.Property(x => x.Explanation).HasMaxLength(4000);
            e.Property(x => x.Marks).HasPrecision(6, 2);
            e.Property(x => x.ImageKey).HasMaxLength(200);
            e.Property(x => x.ImageContentType).HasMaxLength(100);
            e.HasIndex(x => new { x.CategoryId, x.IsDeleted, x.IsActive });
            e.HasOne(x => x.Category).WithMany().HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
        });
        b.Entity<QuestionOption>(e =>
        {
            e.ToTable("question_options");
            e.Property(x => x.Text).HasMaxLength(1000).IsRequired();
            e.HasIndex(x => x.QuestionId);
            // At most one correct option per question.
            e.HasIndex(x => x.QuestionId).HasDatabaseName("ux_question_options_one_correct").IsUnique().HasFilter("is_correct");
            e.HasOne(x => x.Question).WithMany(q => q.Options).HasForeignKey(x => x.QuestionId).OnDelete(DeleteBehavior.Cascade);
        });
        b.Entity<Quiz>(e =>
        {
            e.ToTable("quizzes", t =>
            {
                t.HasCheckConstraint("ck_quizzes_duration", "duration_minutes BETWEEN 1 AND 600");
                t.HasCheckConstraint("ck_quizzes_question_count", "question_count BETWEEN 1 AND 500");
                t.HasCheckConstraint("ck_quizzes_negative", "negative_marks >= 0");
            });
            e.Property(x => x.Title).HasMaxLength(200).IsRequired();
            e.Property(x => x.Description).HasMaxLength(2000);
            e.Property(x => x.MarksPerQuestion).HasPrecision(6, 2);
            e.Property(x => x.NegativeMarks).HasPrecision(6, 2);
            e.HasOne(x => x.Category).WithMany().HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
        });
        b.Entity<QuizQuestion>(e =>
        {
            e.ToTable("quiz_questions");
            e.HasKey(x => new { x.QuizId, x.QuestionId });
            e.HasOne(x => x.Quiz).WithMany(q => q.Questions).HasForeignKey(x => x.QuizId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Question).WithMany().HasForeignKey(x => x.QuestionId).OnDelete(DeleteBehavior.Cascade);
        });
        b.Entity<QuizAttempt>(e =>
        {
            e.ToTable("quiz_attempts");
            e.Property(x => x.QuizTitle).HasMaxLength(200);
            e.Property(x => x.NegativeMarks).HasPrecision(6, 2);
            e.Property(x => x.Score).HasPrecision(10, 2);
            e.Property(x => x.MaxScore).HasPrecision(10, 2);
            e.HasIndex(x => new { x.CompetitionId, x.StudentId, x.QuizId }).IsUnique();
            e.HasIndex(x => x.StudentId);
            e.HasIndex(x => new { x.Status, x.ExpiresAtUtc });
            e.HasOne(x => x.Quiz).WithMany().HasForeignKey(x => x.QuizId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Competition).WithMany().HasForeignKey(x => x.CompetitionId).OnDelete(DeleteBehavior.Restrict);
            Student(e, nameof(QuizAttempt.StudentId));
        });
        b.Entity<QuizAttemptQuestion>(e =>
        {
            e.ToTable("quiz_attempt_questions");
            e.Property(x => x.Snapshot).HasColumnType("jsonb");
            e.Property(x => x.AwardedMarks).HasPrecision(6, 2);
            e.HasIndex(x => new { x.AttemptId, x.OrderNo }).IsUnique();
            e.HasOne(x => x.Attempt).WithMany(a => a.Questions).HasForeignKey(x => x.AttemptId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne<Question>().WithMany().HasForeignKey(x => x.QuestionId).OnDelete(DeleteBehavior.SetNull);
        });

        b.Entity<ProctoringSession>(e =>
        {
            e.ToTable("proctoring_sessions");
            e.Property(x => x.ConsentVersion).HasMaxLength(20);
            e.Property(x => x.LastCameraStatus).HasMaxLength(50);
            e.Property(x => x.UserAgent).HasMaxLength(1000);
            e.Property(x => x.IpAddress).HasMaxLength(64);
            e.HasIndex(x => new { x.StudentId, x.StartedAtUtc });
            e.HasIndex(x => x.EndedAtUtc);
            Student(e, nameof(ProctoringSession.StudentId), DeleteBehavior.Cascade);
        });
        b.Entity<ViolationRule>(e =>
        {
            e.ToTable("violation_rules", t => t.HasCheckConstraint("ck_violation_rules_thresholds",
                "flag_after >= 0 AND terminate_after >= 0 AND suspend_after >= 0 AND (suspend_minutes IS NULL OR suspend_minutes > 0)"));
            e.Property(x => x.Code).HasMaxLength(50).IsRequired();
            e.Property(x => x.Name).HasMaxLength(150).IsRequired();
            e.Property(x => x.Description).HasMaxLength(1000);
            e.HasIndex(x => x.Code).IsUnique();
        });
        b.Entity<ViolationEvent>(e =>
        {
            e.ToTable("violation_events");
            e.Property(x => x.RuleCode).HasMaxLength(50).IsRequired();
            e.Property(x => x.Message).HasMaxLength(1000);
            e.Property(x => x.Details).HasColumnType("jsonb");
            e.Property(x => x.ReviewNote).HasMaxLength(2000);
            e.HasIndex(x => new { x.StudentId, x.OccurredAtUtc });
            e.HasIndex(x => new { x.CompetitionId, x.StudentId, x.RuleCode });
            e.HasIndex(x => new { x.ReviewStatus, x.OccurredAtUtc });
            Student(e, nameof(ViolationEvent.StudentId));
            e.HasOne<ProctoringSession>().WithMany().HasForeignKey(x => x.ProctoringSessionId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne<GameAttempt>().WithMany().HasForeignKey(x => x.GameAttemptId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne<QuizAttempt>().WithMany().HasForeignKey(x => x.QuizAttemptId).OnDelete(DeleteBehavior.SetNull);
        });
        b.Entity<EvidenceFile>(e =>
        {
            e.ToTable("evidence_files", t => t.HasCheckConstraint("ck_evidence_size", "size_bytes > 0"));
            e.Property(x => x.StorageKey).HasMaxLength(300).IsRequired();
            e.Property(x => x.ContentType).HasMaxLength(100).IsRequired();
            e.Property(x => x.Sha256).HasMaxLength(64).IsRequired();
            e.Property(x => x.DeletedBy).HasMaxLength(256);
            e.HasIndex(x => x.ViolationEventId);
            e.HasIndex(x => x.StudentId);
            e.HasIndex(x => new { x.DeletedAtUtc, x.RetainUntilUtc });
            e.HasOne(x => x.ViolationEvent).WithMany(v => v.Evidence).HasForeignKey(x => x.ViolationEventId).OnDelete(DeleteBehavior.Cascade);
        });
        b.Entity<StudentBan>(e =>
        {
            e.ToTable("student_bans");
            e.Property(x => x.Reason).HasMaxLength(2000).IsRequired();
            e.Property(x => x.RuleCode).HasMaxLength(50);
            e.Property(x => x.LiftReason).HasMaxLength(2000);
            e.HasIndex(x => new { x.StudentId, x.LiftedAtUtc });
            Student(e, nameof(StudentBan.StudentId));
            e.HasOne<ViolationEvent>().WithMany().HasForeignKey(x => x.ViolationEventId).OnDelete(DeleteBehavior.SetNull);
        });
        b.Entity<Appeal>(e =>
        {
            e.ToTable("appeals");
            e.Property(x => x.Message).HasMaxLength(4000).IsRequired();
            e.Property(x => x.DecisionReason).HasMaxLength(2000);
            e.HasIndex(x => new { x.Status, x.SubmittedAtUtc });
            e.HasIndex(x => x.StudentId);
            // Only one open appeal per ban.
            e.HasIndex(x => x.BanId).HasDatabaseName("ux_appeals_one_pending").IsUnique().HasFilter("status = 'Pending'");
            e.HasOne(x => x.Ban).WithMany(bn => bn.Appeals).HasForeignKey(x => x.BanId).OnDelete(DeleteBehavior.Cascade);
            e.ToTable(t => t.HasCheckConstraint("ck_appeals_decision",
                "status = 'Pending' OR (decided_at_utc IS NOT NULL AND decision_reason IS NOT NULL)"));
        });
        b.Entity<AppealNote>(e =>
        {
            e.ToTable("appeal_notes");
            e.Property(x => x.AdminName).HasMaxLength(256);
            e.Property(x => x.Note).HasMaxLength(4000).IsRequired();
            e.HasOne(x => x.Appeal).WithMany(a => a.Notes).HasForeignKey(x => x.AppealId).OnDelete(DeleteBehavior.Cascade);
        });

        ApplyConventions(b);
    }

    /// <summary>snake_case column names and string-stored enums for every table except the legacy "ExceptionLogs".</summary>
    private static void ApplyConventions(ModelBuilder b)
    {
        foreach (var entity in b.Model.GetEntityTypes())
        {
            if (entity.ClrType == typeof(ExceptionLog)) continue;
            foreach (var p in entity.GetProperties())
            {
                p.SetColumnName(Snake(p.Name));
                var type = Nullable.GetUnderlyingType(p.ClrType) ?? p.ClrType;
                if (type.IsEnum)
                {
                    p.SetProviderClrType(typeof(string));
                    p.SetMaxLength(20);
                }
            }
        }
    }

    private static string Snake(string name) =>
        Regex.Replace(name, "(?<=[a-z0-9])(?=[A-Z])", "_").ToLowerInvariant();
}
