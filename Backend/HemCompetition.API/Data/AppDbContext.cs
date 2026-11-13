using HemCompetition.API.Domain;
using Microsoft.EntityFrameworkCore;

namespace HemCompetition.API.Data
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        public DbSet<ExceptionLog> ExceptionLogs { get; set; }
        public DbSet<AuthUser> AuthUsers { get; set; }

        public DbSet<AuthSession> AuthSessions { get; set; }

        public DbSet<CompetitionSetting> CompetitionSettings { get; set; }

        public DbSet<CompetitionSecurity> CompetitionSecurities { get; set; }

        public DbSet<AdminUser> AdminUsers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AuthUser>(entity =>
            {
                entity.ToTable("auth_users");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .HasColumnName("id");

                entity.Property(x => x.UserId)
                    .HasColumnName("user_id");

                entity.Property(x => x.Username)
                    .HasColumnName("username")
                    .HasMaxLength(256)
                    .IsRequired();

                entity.Property(x => x.Email)
                    .HasColumnName("email")
                    .HasMaxLength(256);

                entity.Property(x => x.PasswordHash)
                    .HasColumnName("password_hash")
                    .IsRequired();

                entity.Property(x => x.SecurityStamp)
                    .HasColumnName("security_stamp")
                    .HasMaxLength(256);

                entity.Property(x => x.PhoneNumber)
                    .HasColumnName("phone_number")
                    .HasMaxLength(50);

                entity.Property(x => x.FirstName)
                    .HasColumnName("first_name")
                    .HasMaxLength(200);

                entity.Property(x => x.LastName)
                    .HasColumnName("last_name")
                    .HasMaxLength(200);

                entity.Property(x => x.EmailId)
                    .HasColumnName("email_id")
                    .HasMaxLength(256);

                entity.Property(x => x.AddressLine1)
                    .HasColumnName("address_line1")
                    .HasMaxLength(500);

                entity.Property(x => x.City)
                    .HasColumnName("city")
                    .HasMaxLength(200);

                entity.Property(x => x.Pincode)
                    .HasColumnName("pincode")
                    .HasMaxLength(20);

                entity.Property(x => x.IsActive)
                    .HasColumnName("is_active");

                entity.Property(x => x.EmailConfirmed)
                    .HasColumnName("email_confirmed");

                entity.Property(x => x.PhoneNumberConfirmed)
                    .HasColumnName("phone_number_confirmed");

                entity.Property(x => x.TwoFactorEnabled)
                    .HasColumnName("two_factor_enabled");

                entity.Property(x => x.LockoutEnabled)
                    .HasColumnName("lockout_enabled");

                entity.Property(x => x.LockoutEndDateUtc)
                    .HasColumnName("lockout_end_date_utc");

                entity.Property(x => x.AccessFailedCount)
                    .HasColumnName("access_failed_count");

                entity.Property(x => x.IsUserDeactivated)
                    .HasColumnName("is_user_deactivated");

                entity.Property(x => x.CreatedAtUtc)
                    .HasColumnName("created_at_utc");

                entity.Property(x => x.UpdatedAtUtc)
                    .HasColumnName("updated_at_utc");

                entity.Property(x => x.LastLoginAtUtc)
                    .HasColumnName("last_login_at_utc");


                entity.HasIndex(x => x.Username)
                    .IsUnique();

                entity.HasIndex(x => x.UserId)
                    .IsUnique();
            });
            modelBuilder.Entity<AuthSession>(entity =>
            {
                entity.ToTable("auth_sessions");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .HasColumnName("id");

                entity.Property(x => x.UserId)
                    .HasColumnName("user_id");

                entity.Property(x => x.SessionId)
                    .HasColumnName("session_id")
                    .IsRequired();

                entity.Property(x => x.CreatedAtUtc)
                    .HasColumnName("created_at_utc");

                entity.Property(x => x.LastActivityUtc)
                    .HasColumnName("last_activity_utc");

                entity.Property(x => x.LastHeartbeatUtc)
                    .HasColumnName("last_heartbeat_utc");

                entity.Property(x => x.IsActive)
                    .HasColumnName("is_active");

                entity.Property(x => x.IpAddress)
                    .HasColumnName("ip_address")
                    .HasMaxLength(100);

                entity.Property(x => x.UserAgent)
                    .HasColumnName("user_agent")
                    .HasMaxLength(1000);

                entity.Property(x => x.TerminatedAtUtc)
                    .HasColumnName("terminated_at_utc");

                entity.HasIndex(x => x.SessionId)
                    .IsUnique();

                entity.HasIndex(x => x.UserId);

                entity.HasIndex(x => new
                {
                    x.UserId,
                    x.IsActive
                });
            });
            modelBuilder.Entity<CompetitionSetting>(entity =>
            {
                entity.ToTable("competition_settings");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .HasColumnName("id");

                entity.Property(x => x.CompetitionName)
                    .HasColumnName("competition_name");

                entity.Property(x => x.StartDateTimeUtc)
                    .HasColumnName("start_date_time_utc");

                entity.Property(x => x.EndDateTimeUtc)
                    .HasColumnName("end_date_time_utc");

                entity.Property(x => x.IsActive)
                    .HasColumnName("is_active");

                entity.Property(x => x.CreatedAtUtc)
                    .HasColumnName("created_at_utc");

                entity.Property(x => x.UpdatedAtUtc)
                    .HasColumnName("updated_at_utc");
            });
            modelBuilder.Entity<CompetitionSecurity>(entity =>
            {
                entity.ToTable("competition_security");

                entity.HasKey(x => x.Id);

                entity.HasIndex(x => x.UserId)
                    .IsUnique();

                entity.Property(x => x.Id)
                    .HasColumnName("id");

                entity.Property(x => x.UserId)
                    .HasColumnName("user_id");

                entity.Property(x => x.ConsentVersion)
                    .HasColumnName("consent_version");

                entity.Property(x => x.TermsAccepted)
                    .HasColumnName("terms_accepted");

                entity.Property(x => x.TermsAcceptedAtUtc)
                    .HasColumnName("terms_accepted_at_utc");

                entity.Property(x => x.Status)
                    .HasColumnName("status");

                entity.Property(x => x.ViolationType)
                    .HasColumnName("violation_type");

                entity.Property(x => x.ViolationMessage)
                    .HasColumnName("violation_message");

                entity.Property(x => x.ViolationCount)
                    .HasColumnName("violation_count");

                entity.Property(x => x.DisqualifiedAtUtc)
                    .HasColumnName("disqualified_at_utc");

                entity.Property(x => x.LastCameraStatus)
                    .HasColumnName("last_camera_status");

                entity.Property(x => x.LastPersonCount)
                    .HasColumnName("last_person_count");

                entity.Property(x => x.LastViolationAtUtc)
                    .HasColumnName("last_violation_at_utc");

                entity.Property(x => x.CreatedAtUtc)
                    .HasColumnName("created_at_utc");

                entity.Property(x => x.UpdatedAtUtc)
                    .HasColumnName("updated_at_utc");
            });
            modelBuilder.Entity<AdminUser>(entity =>
            {
                entity.ToTable("admin_users");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .HasColumnName("id");

                entity.Property(x => x.Username)
                    .HasColumnName("username")
                    .HasMaxLength(256)
                    .IsRequired();

                entity.Property(x => x.PasswordHash)
                    .HasColumnName("password_hash")
                    .IsRequired();

                entity.Property(x => x.DisplayName)
                    .HasColumnName("display_name")
                    .HasMaxLength(256);

                entity.Property(x => x.IsActive)
                    .HasColumnName("is_active");

                entity.Property(x => x.CreatedAtUtc)
                    .HasColumnName("created_at_utc");

                entity.Property(x => x.UpdatedAtUtc)
                    .HasColumnName("updated_at_utc");

                entity.Property(x => x.LastLoginAtUtc)
                    .HasColumnName("last_login_at_utc");

                entity.Property(x => x.AccessFailedCount)
                    .HasColumnName("access_failed_count");

                entity.HasIndex(x => x.Username)
                    .IsUnique();
            });

            ConfigurePlatform(modelBuilder);
        }
    }
}