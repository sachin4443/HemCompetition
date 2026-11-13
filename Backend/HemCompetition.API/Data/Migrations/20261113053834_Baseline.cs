using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HemCompetition.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class Baseline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "admin_users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    username = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    display_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_login_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    access_failed_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "auth_sessions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_activity_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_heartbeat_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    ip_address = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    user_agent = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    terminated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_auth_sessions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "auth_users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    username = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    security_stamp = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    phone_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    first_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    last_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    email_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    address_line1 = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    city = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    pincode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    phone_number_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    lockout_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    lockout_end_date_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    access_failed_count = table.Column<int>(type: "integer", nullable: false),
                    is_user_deactivated = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_login_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_auth_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "competition_security",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    consent_version = table.Column<string>(type: "text", nullable: true),
                    terms_accepted = table.Column<bool>(type: "boolean", nullable: false),
                    terms_accepted_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    violation_type = table.Column<string>(type: "text", nullable: true),
                    violation_message = table.Column<string>(type: "text", nullable: true),
                    violation_count = table.Column<int>(type: "integer", nullable: false),
                    disqualified_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_camera_status = table.Column<string>(type: "text", nullable: true),
                    last_person_count = table.Column<int>(type: "integer", nullable: true),
                    last_violation_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_competition_security", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "competition_settings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    competition_name = table.Column<string>(type: "text", nullable: false),
                    start_date_time_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_date_time_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_competition_settings", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ExceptionLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Message = table.Column<string>(type: "text", nullable: true),
                    StackTrace = table.Column<string>(type: "text", nullable: true),
                    RequestPath = table.Column<string>(type: "text", nullable: true),
                    DeviceType = table.Column<string>(type: "text", nullable: true),
                    IpAddress = table.Column<string>(type: "text", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExceptionLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_admin_users_username",
                table: "admin_users",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_auth_sessions_session_id",
                table: "auth_sessions",
                column: "session_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_auth_sessions_user_id",
                table: "auth_sessions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_auth_sessions_user_id_is_active",
                table: "auth_sessions",
                columns: new[] { "user_id", "is_active" });

            migrationBuilder.CreateIndex(
                name: "IX_auth_users_user_id",
                table: "auth_users",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_auth_users_username",
                table: "auth_users",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_competition_security_user_id",
                table: "competition_security",
                column: "user_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "admin_users");

            migrationBuilder.DropTable(
                name: "auth_sessions");

            migrationBuilder.DropTable(
                name: "auth_users");

            migrationBuilder.DropTable(
                name: "competition_security");

            migrationBuilder.DropTable(
                name: "competition_settings");

            migrationBuilder.DropTable(
                name: "ExceptionLogs");
        }
    }
}
