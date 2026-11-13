using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HemCompetition.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class PlatformSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "competition_settings",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "proctoring_required",
                table: "competition_settings",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "show_leaderboard",
                table: "competition_settings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "show_results_to_students",
                table: "competition_settings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "admin_users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "lockout_end_utc",
                table: "admin_users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "must_change_password",
                table: "admin_users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "role_id",
                table: "admin_users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_auth_users_user_id",
                table: "auth_users",
                column: "user_id");

            migrationBuilder.CreateTable(
                name: "admin_roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_system = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "audit_logs",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    actor_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    actor_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    actor_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    entity_type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    entity_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    details = table.Column<string>(type: "jsonb", nullable: true),
                    ip_address = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_logs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "game_categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    key = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    student_id = table.Column<long>(type: "bigint", nullable: true),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    body = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    category = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by_admin_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.id);
                    table.ForeignKey(
                        name: "FK_notifications_auth_users_student_id",
                        column: x => x.student_id,
                        principalTable: "auth_users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "proctoring_sessions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    student_id = table.Column<long>(type: "bigint", nullable: false),
                    competition_id = table.Column<Guid>(type: "uuid", nullable: true),
                    auth_session_id = table.Column<Guid>(type: "uuid", nullable: true),
                    consent_version = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    evidence_consent = table.Column<bool>(type: "boolean", nullable: false),
                    video_consent = table.Column<bool>(type: "boolean", nullable: false),
                    started_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ended_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_seen_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_camera_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    last_person_count = table.Column<int>(type: "integer", nullable: true),
                    user_agent = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ip_address = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_proctoring_sessions", x => x.id);
                    table.ForeignKey(
                        name: "FK_proctoring_sessions_auth_users_student_id",
                        column: x => x.student_id,
                        principalTable: "auth_users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "quiz_categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quiz_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    token_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    student_id = table.Column<long>(type: "bigint", nullable: true),
                    admin_id = table.Column<Guid>(type: "uuid", nullable: true),
                    session_id = table.Column<Guid>(type: "uuid", nullable: true),
                    family_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expires_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    used_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    revoked_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens", x => x.id);
                    table.CheckConstraint("ck_refresh_tokens_subject", "(student_id IS NOT NULL) <> (admin_id IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_refresh_tokens_admin_users_admin_id",
                        column: x => x.admin_id,
                        principalTable: "admin_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_refresh_tokens_auth_users_student_id",
                        column: x => x.student_id,
                        principalTable: "auth_users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "system_settings",
                columns: table => new
                {
                    key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    value = table.Column<string>(type: "text", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_by = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_system_settings", x => x.key);
                });

            migrationBuilder.CreateTable(
                name: "violation_rules",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    severity = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    capture_image = table.Column<bool>(type: "boolean", nullable: false),
                    capture_video = table.Column<bool>(type: "boolean", nullable: false),
                    flag_after = table.Column<int>(type: "integer", nullable: false),
                    terminate_after = table.Column<int>(type: "integer", nullable: false),
                    suspend_after = table.Column<int>(type: "integer", nullable: false),
                    suspend_minutes = table.Column<int>(type: "integer", nullable: true),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_violation_rules", x => x.id);
                    table.CheckConstraint("ck_violation_rules_thresholds", "flag_after >= 0 AND terminate_after >= 0 AND suspend_after >= 0 AND (suspend_minutes IS NULL OR suspend_minutes > 0)");
                });

            migrationBuilder.CreateTable(
                name: "word_bank",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    word = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    clue = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    topic = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    difficulty = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_word_bank", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "admin_role_permissions",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    permission = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin_role_permissions", x => new { x.role_id, x.permission });
                    table.ForeignKey(
                        name: "FK_admin_role_permissions_admin_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "admin_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "games",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    category_id = table.Column<int>(type: "integer", nullable: false),
                    key = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    engine_key = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    difficulty = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    rounds = table.Column<int>(type: "integer", nullable: false),
                    points_per_round = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: false),
                    time_limit_seconds = table.Column<int>(type: "integer", nullable: true),
                    config = table.Column<string>(type: "jsonb", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_games", x => x.id);
                    table.CheckConstraint("ck_games_points", "points_per_round >= 0");
                    table.CheckConstraint("ck_games_rounds", "rounds BETWEEN 1 AND 50");
                    table.ForeignKey(
                        name: "FK_games_game_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "game_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "notification_reads",
                columns: table => new
                {
                    notification_id = table.Column<long>(type: "bigint", nullable: false),
                    student_id = table.Column<long>(type: "bigint", nullable: false),
                    read_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification_reads", x => new { x.notification_id, x.student_id });
                    table.ForeignKey(
                        name: "FK_notification_reads_auth_users_student_id",
                        column: x => x.student_id,
                        principalTable: "auth_users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_notification_reads_notifications_notification_id",
                        column: x => x.notification_id,
                        principalTable: "notifications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "questions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    category_id = table.Column<int>(type: "integer", nullable: false),
                    text = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    explanation = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    difficulty = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    marks = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    image_key = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    image_content_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_questions", x => x.id);
                    table.CheckConstraint("ck_questions_marks", "marks > 0");
                    table.ForeignKey(
                        name: "FK_questions_quiz_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "quiz_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "quizzes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    category_id = table.Column<int>(type: "integer", nullable: true),
                    selection_mode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    difficulty = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    question_count = table.Column<int>(type: "integer", nullable: false),
                    duration_minutes = table.Column<int>(type: "integer", nullable: false),
                    marks_per_question = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: true),
                    negative_marks = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    shuffle_questions = table.Column<bool>(type: "boolean", nullable: false),
                    shuffle_options = table.Column<bool>(type: "boolean", nullable: false),
                    is_published = table.Column<bool>(type: "boolean", nullable: false),
                    published_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    version = table.Column<int>(type: "integer", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quizzes", x => x.id);
                    table.CheckConstraint("ck_quizzes_duration", "duration_minutes BETWEEN 1 AND 600");
                    table.CheckConstraint("ck_quizzes_negative", "negative_marks >= 0");
                    table.CheckConstraint("ck_quizzes_question_count", "question_count BETWEEN 1 AND 500");
                    table.ForeignKey(
                        name: "FK_quizzes_quiz_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "quiz_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "game_attempts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    competition_id = table.Column<Guid>(type: "uuid", nullable: false),
                    student_id = table.Column<long>(type: "bigint", nullable: false),
                    game_id = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    difficulty = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    config_snapshot = table.Column<string>(type: "jsonb", nullable: false),
                    total_rounds = table.Column<int>(type: "integer", nullable: false),
                    current_round = table.Column<int>(type: "integer", nullable: false),
                    correct_count = table.Column<int>(type: "integer", nullable: false),
                    score = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    max_score = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    time_taken_seconds = table.Column<int>(type: "integer", nullable: false),
                    is_flagged = table.Column<bool>(type: "boolean", nullable: false),
                    started_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deadline_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_attempts", x => x.id);
                    table.ForeignKey(
                        name: "FK_game_attempts_auth_users_student_id",
                        column: x => x.student_id,
                        principalTable: "auth_users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_game_attempts_competition_settings_competition_id",
                        column: x => x.competition_id,
                        principalTable: "competition_settings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_game_attempts_games_game_id",
                        column: x => x.game_id,
                        principalTable: "games",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "question_options",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    question_id = table.Column<int>(type: "integer", nullable: false),
                    text = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    is_correct = table.Column<bool>(type: "boolean", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_question_options", x => x.id);
                    table.ForeignKey(
                        name: "FK_question_options_questions_question_id",
                        column: x => x.question_id,
                        principalTable: "questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "quiz_attempts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    competition_id = table.Column<Guid>(type: "uuid", nullable: false),
                    student_id = table.Column<long>(type: "bigint", nullable: false),
                    quiz_id = table.Column<int>(type: "integer", nullable: false),
                    quiz_version = table.Column<int>(type: "integer", nullable: false),
                    quiz_title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    negative_marks = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    started_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expires_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    submitted_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    score = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    max_score = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    correct_count = table.Column<int>(type: "integer", nullable: false),
                    incorrect_count = table.Column<int>(type: "integer", nullable: false),
                    unanswered_count = table.Column<int>(type: "integer", nullable: false),
                    time_taken_seconds = table.Column<int>(type: "integer", nullable: false),
                    is_flagged = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quiz_attempts", x => x.id);
                    table.ForeignKey(
                        name: "FK_quiz_attempts_auth_users_student_id",
                        column: x => x.student_id,
                        principalTable: "auth_users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_quiz_attempts_competition_settings_competition_id",
                        column: x => x.competition_id,
                        principalTable: "competition_settings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_quiz_attempts_quizzes_quiz_id",
                        column: x => x.quiz_id,
                        principalTable: "quizzes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "quiz_questions",
                columns: table => new
                {
                    quiz_id = table.Column<int>(type: "integer", nullable: false),
                    question_id = table.Column<int>(type: "integer", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quiz_questions", x => new { x.quiz_id, x.question_id });
                    table.ForeignKey(
                        name: "FK_quiz_questions_questions_question_id",
                        column: x => x.question_id,
                        principalTable: "questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_quiz_questions_quizzes_quiz_id",
                        column: x => x.quiz_id,
                        principalTable: "quizzes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "game_attempt_rounds",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    attempt_id = table.Column<Guid>(type: "uuid", nullable: false),
                    round_no = table.Column<int>(type: "integer", nullable: false),
                    puzzle = table.Column<string>(type: "jsonb", nullable: false),
                    solution = table.Column<string>(type: "jsonb", nullable: false),
                    submission = table.Column<string>(type: "jsonb", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    wrong_submissions = table.Column<int>(type: "integer", nullable: false),
                    score = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: false),
                    presented_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    answered_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    time_taken_seconds = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_attempt_rounds", x => x.id);
                    table.ForeignKey(
                        name: "FK_game_attempt_rounds_game_attempts_attempt_id",
                        column: x => x.attempt_id,
                        principalTable: "game_attempts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "quiz_attempt_questions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    attempt_id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_no = table.Column<int>(type: "integer", nullable: false),
                    question_id = table.Column<int>(type: "integer", nullable: true),
                    snapshot = table.Column<string>(type: "jsonb", nullable: false),
                    selected_option_id = table.Column<int>(type: "integer", nullable: true),
                    is_correct = table.Column<bool>(type: "boolean", nullable: true),
                    awarded_marks = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    answered_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quiz_attempt_questions", x => x.id);
                    table.ForeignKey(
                        name: "FK_quiz_attempt_questions_questions_question_id",
                        column: x => x.question_id,
                        principalTable: "questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_quiz_attempt_questions_quiz_attempts_attempt_id",
                        column: x => x.attempt_id,
                        principalTable: "quiz_attempts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "violation_events",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    student_id = table.Column<long>(type: "bigint", nullable: false),
                    competition_id = table.Column<Guid>(type: "uuid", nullable: true),
                    proctoring_session_id = table.Column<Guid>(type: "uuid", nullable: true),
                    rule_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    severity = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    message = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    details = table.Column<string>(type: "jsonb", nullable: true),
                    game_attempt_id = table.Column<Guid>(type: "uuid", nullable: true),
                    quiz_attempt_id = table.Column<Guid>(type: "uuid", nullable: true),
                    occurrence_no = table.Column<int>(type: "integer", nullable: false),
                    action_taken = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    occurred_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    review_status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    reviewed_by_admin_id = table.Column<Guid>(type: "uuid", nullable: true),
                    reviewed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    review_note = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_violation_events", x => x.id);
                    table.ForeignKey(
                        name: "FK_violation_events_auth_users_student_id",
                        column: x => x.student_id,
                        principalTable: "auth_users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_violation_events_game_attempts_game_attempt_id",
                        column: x => x.game_attempt_id,
                        principalTable: "game_attempts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_violation_events_proctoring_sessions_proctoring_session_id",
                        column: x => x.proctoring_session_id,
                        principalTable: "proctoring_sessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_violation_events_quiz_attempts_quiz_attempt_id",
                        column: x => x.quiz_attempt_id,
                        principalTable: "quiz_attempts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "evidence_files",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    violation_event_id = table.Column<Guid>(type: "uuid", nullable: false),
                    student_id = table.Column<long>(type: "bigint", nullable: false),
                    kind = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    storage_key = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    content_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    size_bytes = table.Column<long>(type: "bigint", nullable: false),
                    sha256 = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    captured_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    retain_until_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_evidence_files", x => x.id);
                    table.CheckConstraint("ck_evidence_size", "size_bytes > 0");
                    table.ForeignKey(
                        name: "FK_evidence_files_violation_events_violation_event_id",
                        column: x => x.violation_event_id,
                        principalTable: "violation_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "student_bans",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    student_id = table.Column<long>(type: "bigint", nullable: false),
                    competition_id = table.Column<Guid>(type: "uuid", nullable: true),
                    type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    source = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    reason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    rule_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    violation_event_id = table.Column<Guid>(type: "uuid", nullable: true),
                    starts_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ends_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by_admin_id = table.Column<Guid>(type: "uuid", nullable: true),
                    lifted_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    lifted_by_admin_id = table.Column<Guid>(type: "uuid", nullable: true),
                    lift_reason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_bans", x => x.id);
                    table.ForeignKey(
                        name: "FK_student_bans_auth_users_student_id",
                        column: x => x.student_id,
                        principalTable: "auth_users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_student_bans_violation_events_violation_event_id",
                        column: x => x.violation_event_id,
                        principalTable: "violation_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "appeals",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ban_id = table.Column<Guid>(type: "uuid", nullable: false),
                    student_id = table.Column<long>(type: "bigint", nullable: false),
                    message = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    submitted_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    decided_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    decided_by_admin_id = table.Column<Guid>(type: "uuid", nullable: true),
                    decision_reason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appeals", x => x.id);
                    table.CheckConstraint("ck_appeals_decision", "status = 'Pending' OR (decided_at_utc IS NOT NULL AND decision_reason IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_appeals_student_bans_ban_id",
                        column: x => x.ban_id,
                        principalTable: "student_bans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "appeal_notes",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    appeal_id = table.Column<Guid>(type: "uuid", nullable: false),
                    admin_id = table.Column<Guid>(type: "uuid", nullable: false),
                    admin_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    note = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appeal_notes", x => x.id);
                    table.ForeignKey(
                        name: "FK_appeal_notes_appeals_appeal_id",
                        column: x => x.appeal_id,
                        principalTable: "appeals",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_admin_users_role_id",
                table: "admin_users",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_admin_roles_name",
                table: "admin_roles",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_appeal_notes_appeal_id",
                table: "appeal_notes",
                column: "appeal_id");

            migrationBuilder.CreateIndex(
                name: "IX_appeals_status_submitted_at_utc",
                table: "appeals",
                columns: new[] { "status", "submitted_at_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_appeals_student_id",
                table: "appeals",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "ux_appeals_one_pending",
                table: "appeals",
                column: "ban_id",
                unique: true,
                filter: "status = 'Pending'");

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_created_at_utc",
                table: "audit_logs",
                column: "created_at_utc");

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_entity_type_entity_id",
                table: "audit_logs",
                columns: new[] { "entity_type", "entity_id" });

            migrationBuilder.CreateIndex(
                name: "IX_evidence_files_deleted_at_utc_retain_until_utc",
                table: "evidence_files",
                columns: new[] { "deleted_at_utc", "retain_until_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_evidence_files_student_id",
                table: "evidence_files",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "IX_evidence_files_violation_event_id",
                table: "evidence_files",
                column: "violation_event_id");

            migrationBuilder.CreateIndex(
                name: "IX_game_attempt_rounds_attempt_id_round_no",
                table: "game_attempt_rounds",
                columns: new[] { "attempt_id", "round_no" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_game_attempts_competition_id_student_id_game_id",
                table: "game_attempts",
                columns: new[] { "competition_id", "student_id", "game_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_game_attempts_game_id_status",
                table: "game_attempts",
                columns: new[] { "game_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_game_attempts_student_id",
                table: "game_attempts",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "IX_game_categories_key",
                table: "game_categories",
                column: "key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_games_category_id",
                table: "games",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_games_key",
                table: "games",
                column: "key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_notification_reads_student_id",
                table: "notification_reads",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_student_id_created_at_utc",
                table: "notifications",
                columns: new[] { "student_id", "created_at_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_proctoring_sessions_ended_at_utc",
                table: "proctoring_sessions",
                column: "ended_at_utc");

            migrationBuilder.CreateIndex(
                name: "IX_proctoring_sessions_student_id_started_at_utc",
                table: "proctoring_sessions",
                columns: new[] { "student_id", "started_at_utc" });

            migrationBuilder.CreateIndex(
                name: "ux_question_options_one_correct",
                table: "question_options",
                column: "question_id",
                unique: true,
                filter: "is_correct");

            migrationBuilder.CreateIndex(
                name: "IX_questions_category_id_is_deleted_is_active",
                table: "questions",
                columns: new[] { "category_id", "is_deleted", "is_active" });

            migrationBuilder.CreateIndex(
                name: "IX_quiz_attempt_questions_attempt_id_order_no",
                table: "quiz_attempt_questions",
                columns: new[] { "attempt_id", "order_no" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_quiz_attempt_questions_question_id",
                table: "quiz_attempt_questions",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "IX_quiz_attempts_competition_id_student_id_quiz_id",
                table: "quiz_attempts",
                columns: new[] { "competition_id", "student_id", "quiz_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_quiz_attempts_quiz_id",
                table: "quiz_attempts",
                column: "quiz_id");

            migrationBuilder.CreateIndex(
                name: "IX_quiz_attempts_status_expires_at_utc",
                table: "quiz_attempts",
                columns: new[] { "status", "expires_at_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_quiz_attempts_student_id",
                table: "quiz_attempts",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "IX_quiz_categories_name",
                table: "quiz_categories",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_quiz_questions_question_id",
                table: "quiz_questions",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "IX_quizzes_category_id",
                table: "quizzes",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_admin_id",
                table: "refresh_tokens",
                column: "admin_id");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_expires_at_utc",
                table: "refresh_tokens",
                column: "expires_at_utc");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_family_id",
                table: "refresh_tokens",
                column: "family_id");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_student_id",
                table: "refresh_tokens",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_token_hash",
                table: "refresh_tokens",
                column: "token_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_student_bans_student_id_lifted_at_utc",
                table: "student_bans",
                columns: new[] { "student_id", "lifted_at_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_student_bans_violation_event_id",
                table: "student_bans",
                column: "violation_event_id");

            migrationBuilder.CreateIndex(
                name: "IX_violation_events_competition_id_student_id_rule_code",
                table: "violation_events",
                columns: new[] { "competition_id", "student_id", "rule_code" });

            migrationBuilder.CreateIndex(
                name: "IX_violation_events_game_attempt_id",
                table: "violation_events",
                column: "game_attempt_id");

            migrationBuilder.CreateIndex(
                name: "IX_violation_events_proctoring_session_id",
                table: "violation_events",
                column: "proctoring_session_id");

            migrationBuilder.CreateIndex(
                name: "IX_violation_events_quiz_attempt_id",
                table: "violation_events",
                column: "quiz_attempt_id");

            migrationBuilder.CreateIndex(
                name: "IX_violation_events_review_status_occurred_at_utc",
                table: "violation_events",
                columns: new[] { "review_status", "occurred_at_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_violation_events_student_id_occurred_at_utc",
                table: "violation_events",
                columns: new[] { "student_id", "occurred_at_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_violation_rules_code",
                table: "violation_rules",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_word_bank_word",
                table: "word_bank",
                column: "word",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_admin_users_admin_roles_role_id",
                table: "admin_users",
                column: "role_id",
                principalTable: "admin_roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            // ---- data carry-over (existing databases only; no-ops on a fresh one) ----

            // Admin accounts that existed before this migration were seeded with a password that is
            // in the repository history, so they must choose a new one at next login.
            migrationBuilder.Sql("UPDATE admin_users SET must_change_password = TRUE;");

            // Students disqualified under the old single-flag model become reviewable ban records,
            // which is what gives them the appeal option.
            migrationBuilder.Sql(@"
                INSERT INTO student_bans (id, student_id, type, source, reason, rule_code, starts_at_utc)
                SELECT gen_random_uuid(), cs.user_id, 'Ban', 'Automatic',
                       COALESCE(NULLIF(cs.violation_message, ''), 'Disqualified for a security rule violation.'),
                       cs.violation_type,
                       COALESCE(cs.disqualified_at_utc, now())
                FROM competition_security cs
                JOIN auth_users u ON u.user_id = cs.user_id
                WHERE upper(cs.status) = 'DISQUALIFIED';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_admin_users_admin_roles_role_id",
                table: "admin_users");

            migrationBuilder.DropTable(
                name: "admin_role_permissions");

            migrationBuilder.DropTable(
                name: "appeal_notes");

            migrationBuilder.DropTable(
                name: "audit_logs");

            migrationBuilder.DropTable(
                name: "evidence_files");

            migrationBuilder.DropTable(
                name: "game_attempt_rounds");

            migrationBuilder.DropTable(
                name: "notification_reads");

            migrationBuilder.DropTable(
                name: "question_options");

            migrationBuilder.DropTable(
                name: "quiz_attempt_questions");

            migrationBuilder.DropTable(
                name: "quiz_questions");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "system_settings");

            migrationBuilder.DropTable(
                name: "violation_rules");

            migrationBuilder.DropTable(
                name: "word_bank");

            migrationBuilder.DropTable(
                name: "admin_roles");

            migrationBuilder.DropTable(
                name: "appeals");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "questions");

            migrationBuilder.DropTable(
                name: "student_bans");

            migrationBuilder.DropTable(
                name: "violation_events");

            migrationBuilder.DropTable(
                name: "game_attempts");

            migrationBuilder.DropTable(
                name: "proctoring_sessions");

            migrationBuilder.DropTable(
                name: "quiz_attempts");

            migrationBuilder.DropTable(
                name: "games");

            migrationBuilder.DropTable(
                name: "quizzes");

            migrationBuilder.DropTable(
                name: "game_categories");

            migrationBuilder.DropTable(
                name: "quiz_categories");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_auth_users_user_id",
                table: "auth_users");

            migrationBuilder.DropIndex(
                name: "IX_admin_users_role_id",
                table: "admin_users");

            migrationBuilder.DropColumn(
                name: "description",
                table: "competition_settings");

            migrationBuilder.DropColumn(
                name: "proctoring_required",
                table: "competition_settings");

            migrationBuilder.DropColumn(
                name: "show_leaderboard",
                table: "competition_settings");

            migrationBuilder.DropColumn(
                name: "show_results_to_students",
                table: "competition_settings");

            migrationBuilder.DropColumn(
                name: "email",
                table: "admin_users");

            migrationBuilder.DropColumn(
                name: "lockout_end_utc",
                table: "admin_users");

            migrationBuilder.DropColumn(
                name: "must_change_password",
                table: "admin_users");

            migrationBuilder.DropColumn(
                name: "role_id",
                table: "admin_users");
        }
    }
}
