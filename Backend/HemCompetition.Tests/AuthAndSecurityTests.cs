using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using HemCompetition.API.Controllers.Admin;
using HemCompetition.API.Data;
using HemCompetition.API.Domain;
using HemCompetition.API.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql;

namespace HemCompetition.Tests;

public class AuthAndSecurityTests(TestApp app) : IClassFixture<TestApp>
{
    [Fact]
    public async Task Fresh_database_is_migrated_and_seeded()
    {
        await app.DbAsync(async db =>
        {
            Assert.Empty(await db.Database.GetPendingMigrationsAsync());
            Assert.Equal(3, await db.GameCategories.CountAsync());
            Assert.Equal(6, await db.Games.CountAsync());
            Assert.Equal(4, await db.Quizzes.CountAsync(q => !q.IsPublished));
            Assert.Equal(6, await db.AdminRoles.CountAsync());
            Assert.True(await db.ViolationRules.CountAsync() >= 14);
            Assert.True(await db.WordBank.CountAsync() > 100);
        });
    }

    [Fact]
    public async Task Unknown_user_and_wrong_password_are_indistinguishable()
    {
        var id = await app.CreateStudentAsync();
        var client = app.Anonymous();
        var unknown = await client.PostAsJsonAsync("/api/auth/login", app.LoginBody("nobody-here", "x"));
        var wrong = await client.PostAsJsonAsync("/api/auth/login", app.LoginBody($"student{id}", "wrong-password"));
        Assert.Equal(HttpStatusCode.Unauthorized, unknown.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, wrong.StatusCode);
        Assert.Equal(await unknown.Content.ReadAsStringAsync(), await wrong.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Student_login_requires_a_correct_single_use_captcha()
    {
        var id = await app.CreateStudentAsync();
        var client = app.Anonymous();
        var shown = await client.GetJson("/api/auth/captcha");
        Assert.True(shown.GetProperty("enabled").GetBoolean());
        var svg = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(shown.GetProperty("image").GetString()!.Split(',')[1]));
        Assert.DoesNotContain("<text", svg); // the code is drawn, not written in the markup

        // Missing, wrong, and unknown captchas are refused before the password is even looked at.
        var none = await client.PostAsJsonAsync("/api/auth/login", new { usernameOrEmail = $"student{id}", password = TestApp.StudentPassword });
        Assert.Equal("CAPTCHA_INVALID", await none.Code());
        var wrong = await client.PostAsJsonAsync("/api/auth/login", new { usernameOrEmail = $"student{id}", password = TestApp.StudentPassword, captchaId = shown.GetProperty("id").GetString(), captchaAnswer = "abcde" });
        Assert.Equal("CAPTCHA_INVALID", await wrong.Code());
        Assert.Equal(0, await app.DbAsync(db => db.AuthUsers.Where(u => u.UserId == id).Select(u => u.AccessFailedCount).FirstAsync()));

        // A solved captcha works once: replaying it with another password guess fails.
        var body = app.LoginBody($"student{id}", "wrong-password");
        Assert.Equal("INVALID_CREDENTIALS", await (await client.PostAsJsonAsync("/api/auth/login", body)).Code());
        Assert.Equal("CAPTCHA_INVALID", await (await client.PostAsJsonAsync("/api/auth/login", body)).Code());

        (await client.PostAsJsonAsync("/api/auth/login", app.LoginBody($"student{id}"))).EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Account_locks_after_repeated_failures_even_with_the_right_password()
    {
        var id = await app.CreateStudentAsync();
        var client = app.Anonymous();
        for (var i = 0; i < 5; i++)
            await client.PostAsJsonAsync("/api/auth/login", app.LoginBody($"student{id}", "bad"));
        var locked = await client.PostAsJsonAsync("/api/auth/login", app.LoginBody($"student{id}", TestApp.StudentPassword));
        Assert.Equal((HttpStatusCode)423, locked.StatusCode);
    }

    [Fact]
    public async Task Second_login_needs_confirmation_and_then_invalidates_the_first_session()
    {
        var (first, id) = await app.StudentAsync();
        (await first.GetAsync("/api/student/dashboard")).EnsureSuccessStatusCode();

        var second = app.Anonymous();
        var conflict = await second.PostAsJsonAsync("/api/auth/login", app.LoginBody($"student{id}", TestApp.StudentPassword));
        Assert.Equal(HttpStatusCode.Conflict, conflict.StatusCode);
        Assert.Equal("SESSION_ACTIVE", await conflict.Code());
        (await first.GetAsync("/api/student/dashboard")).EnsureSuccessStatusCode(); // still valid

        // Ending the other session requires the password again - a user id alone is not enough.
        var bad = await second.PostAsJsonAsync("/api/auth/login", app.LoginBody($"student{id}", "nope", terminateExisting: true));
        Assert.Equal(HttpStatusCode.Unauthorized, bad.StatusCode);
        (await first.GetAsync("/api/student/dashboard")).EnsureSuccessStatusCode();

        var ok = await second.PostAsJsonAsync("/api/auth/login", app.LoginBody($"student{id}", TestApp.StudentPassword, terminateExisting: true));
        ok.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Unauthorized, (await first.GetAsync("/api/student/dashboard")).StatusCode);
        Assert.True(await app.DbAsync(db => db.ViolationEvents.AnyAsync(v => v.StudentId == id && v.RuleCode == "MULTIPLE_SESSION")));
    }

    [Fact]
    public async Task Refresh_token_rotates_and_reuse_revokes_the_family()
    {
        var id = await app.CreateStudentAsync();
        var handler = app.Factory.Server.CreateHandler();
        async Task<(HttpResponseMessage Response, string? Cookie)> Send(string url, object? body, string? cookie)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost" + url) { Content = JsonContent.Create(body ?? new { }) };
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            if (cookie != null) request.Headers.Add("Cookie", cookie);
            var response = await new HttpMessageInvoker(handler, false).SendAsync(request, default);
            var set = response.Headers.TryGetValues("Set-Cookie", out var values) ? values.FirstOrDefault(v => v.StartsWith("hc_refresh=")) : null;
            return (response, set?.Split(';')[0]);
        }

        var (login, cookie1) = await Send("/api/auth/login", app.LoginBody($"student{id}", TestApp.StudentPassword), null);
        login.EnsureSuccessStatusCode();
        Assert.NotNull(cookie1);
        Assert.Contains("httponly", string.Join(";", login.Headers.GetValues("Set-Cookie")).ToLowerInvariant());
        Assert.Contains("samesite=strict", string.Join(";", login.Headers.GetValues("Set-Cookie")).ToLowerInvariant());

        var (refresh1, cookie2) = await Send("/api/auth/refresh", null, cookie1);
        refresh1.EnsureSuccessStatusCode();
        Assert.NotEqual(cookie1, cookie2);

        // The first cookie was already used: presenting it again is treated as theft.
        var (replay, _) = await Send("/api/auth/refresh", null, cookie1);
        Assert.Equal(HttpStatusCode.Unauthorized, replay.StatusCode);
        var (afterReplay, _) = await Send("/api/auth/refresh", null, cookie2);
        Assert.Equal(HttpStatusCode.Unauthorized, afterReplay.StatusCode);

        // Without the anti-CSRF header the endpoint refuses outright.
        var noHeader = new HttpRequestMessage(HttpMethod.Post, "http://localhost/api/auth/refresh");
        noHeader.Headers.Add("Cookie", cookie2);
        Assert.Equal(HttpStatusCode.BadRequest, (await new HttpMessageInvoker(handler, false).SendAsync(noHeader, default)).StatusCode);
    }

    [Fact]
    public async Task Tokens_only_work_for_the_kind_of_user_they_were_issued_to()
    {
        var (student, _) = await app.StudentAsync();
        var admin = await app.AdminAsync();
        Assert.Equal(HttpStatusCode.Forbidden, (await student.GetAsync("/api/admin/students")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await student.GetAsync("/api/admin/quiz/questions")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await student.PostAsJsonAsync("/api/admin/competitions", new { competitionName = "x" })).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await admin.GetAsync("/api/games")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await admin.GetAsync("/api/student/dashboard")).StatusCode);
    }

    [Theory]
    [InlineData("GET", "/api/games")]
    [InlineData("POST", "/api/games/game24/start")]
    [InlineData("GET", "/api/quiz")]
    [InlineData("GET", "/api/student/dashboard")]
    [InlineData("GET", "/api/student/leaderboard")]
    [InlineData("POST", "/api/proctoring/events")]
    [InlineData("GET", "/api/appeals/mine")]
    [InlineData("GET", "/api/admin/dashboard")]
    [InlineData("GET", "/api/admin/students")]
    [InlineData("GET", "/api/admin/reports/leaderboard")]
    [InlineData("GET", "/api/admin/quiz/questions/export")]
    [InlineData("GET", "/api/admin/proctoring/evidence")]
    [InlineData("GET", "/api/admin-sessions")]
    [InlineData("GET", "/api/admin-dashboard/diagnostics")]
    public async Task Protected_endpoints_reject_anonymous_callers(string method, string url)
    {
        var response = await app.Anonymous().SendAsync(new HttpRequestMessage(new HttpMethod(method), url) { Content = JsonContent.Create(new { }) });
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Forged_and_tampered_tokens_are_rejected()
    {
        var (student, _) = await app.StudentAsync();
        var token = student.DefaultRequestHeaders.Authorization!.Parameter!;
        var parts = token.Split('.');
        var payload = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(parts[1].Replace('-', '+').Replace('_', '/').PadRight((parts[1].Length + 3) / 4 * 4, '=')));
        var forged = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(payload.Replace("\"student\"", "\"admin\""))).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        var client = app.Anonymous();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{parts[0]}.{forged}.{parts[2]}");
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/admin/dashboard")).StatusCode);
    }

    [Fact]
    public void Every_admin_action_declares_a_permission()
    {
        var missing = typeof(AdminAuthController).Assembly.GetTypes()
            .Where(t => typeof(ControllerBase).IsAssignableFrom(t) && !t.IsAbstract && t != typeof(AdminAuthController))
            .Where(t => t.Namespace!.EndsWith(".Admin") || t.GetCustomAttribute<RouteAttribute>()!.Template.StartsWith("api/admin"))
            .SelectMany(t => t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(m => m.GetCustomAttributes<Microsoft.AspNetCore.Mvc.Routing.HttpMethodAttribute>().Any())
                .Where(m => !m.GetCustomAttributes<RequirePermissionAttribute>().Any() && !t.GetCustomAttributes<RequirePermissionAttribute>().Any())
                .Select(m => $"{t.Name}.{m.Name}"))
            .ToList();
        Assert.True(missing.Count == 0, "Admin actions without [RequirePermission]: " + string.Join(", ", missing));
    }

    [Fact]
    public void Every_other_action_requires_sign_in_unless_explicitly_public()
    {
        string[] publicActions = ["AuthController.Login", "AuthController.Refresh", "AdminAuthController.Login", "AdminAuthController.Refresh", "CompetitionInfoController.Active", "AuthController.Captcha"];
        var open = typeof(AdminAuthController).Assembly.GetTypes()
            .Where(t => typeof(ControllerBase).IsAssignableFrom(t) && !t.IsAbstract)
            .SelectMany(t => t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(m => m.GetCustomAttributes<Microsoft.AspNetCore.Mvc.Routing.HttpMethodAttribute>().Any())
                .Where(m => !m.GetCustomAttributes<AuthorizeAttribute>().Any() && !t.GetCustomAttributes<AuthorizeAttribute>().Any())
                .Select(m => $"{t.Name}.{m.Name}"))
            .Except(publicActions).ToList();
        Assert.True(open.Count == 0, "Actions reachable without sign-in: " + string.Join(", ", open));
    }

    [Fact]
    public async Task Role_permissions_are_enforced_on_the_server()
    {
        var quizManager = await app.AdminAsync("Quiz Manager");
        var auditor = await app.AdminAsync("Read-only Auditor");
        var reviewer = await app.AdminAsync("Reviewer");

        (await quizManager.GetAsync("/api/admin/quiz/questions")).EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Forbidden, (await quizManager.GetAsync("/api/admin/bans")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await quizManager.GetAsync("/api/admin/proctoring/evidence")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await quizManager.GetAsync("/api/admin/admins")).StatusCode);

        (await auditor.GetAsync("/api/admin/students")).EnsureSuccessStatusCode();
        (await auditor.GetAsync("/api/admin/audit-logs")).EnsureSuccessStatusCode();
        // An auditor must not see answer keys or evidence, or change anything.
        Assert.Equal(HttpStatusCode.Forbidden, (await auditor.GetAsync("/api/admin/quiz/questions")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await auditor.GetAsync("/api/admin/proctoring/evidence")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await auditor.PostAsJsonAsync("/api/admin/bans", new { studentId = 1, type = "Ban", reason = "because" })).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await auditor.PutAsJsonAsync("/api/admin/settings", new { })).StatusCode);

        (await reviewer.GetAsync("/api/admin/appeals")).EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Forbidden, (await reviewer.GetAsync("/api/admin/quiz/questions")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await reviewer.GetAsync("/api/admin/settings")).StatusCode);
    }

    [Fact]
    public async Task Deactivating_an_admin_cuts_off_their_existing_token()
    {
        var super = await app.AdminAsync();
        var victim = await app.AdminAsync("Admin");
        var me = await victim.GetJson("/api/admin/auth/me");
        var id = me.GetProperty("profile").GetProperty("id").GetString();
        var roleId = await app.DbAsync(db => db.AdminRoles.Where(r => r.Name == "Admin").Select(r => r.Id).FirstAsync());
        await super.PutJson($"/api/admin/admins/{id}", new { username = me.GetProperty("profile").GetProperty("username").GetString(), roleId, isActive = false });
        Assert.Equal(HttpStatusCode.Unauthorized, (await victim.GetAsync("/api/admin/students")).StatusCode);
    }

    [Fact]
    public async Task Bootstrap_admin_must_change_password_before_doing_anything()
    {
        var client = app.Anonymous();
        var login = await client.PostJson("/api/admin/auth/login", new { username = "root", password = TestApp.AdminPassword });
        Assert.True(login.GetProperty("profile").GetProperty("mustChangePassword").GetBoolean());
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login.GetProperty("token").GetString());
        var blocked = await client.GetAsync("/api/admin/dashboard");
        Assert.Equal(HttpStatusCode.Forbidden, blocked.StatusCode);
        Assert.Equal("PASSWORD_CHANGE_REQUIRED", await blocked.Code());

        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/api/admin/auth/change-password", new { currentPassword = TestApp.AdminPassword, newPassword = "short" })).StatusCode);
        await client.PostJson("/api/admin/auth/change-password", new { currentPassword = TestApp.AdminPassword, newPassword = "A-Much-Better-Passw0rd" });
        (await client.GetAsync("/api/admin/dashboard")).EnsureSuccessStatusCode();
        // restore for other tests
        await client.PostJson("/api/admin/auth/change-password", new { currentPassword = "A-Much-Better-Passw0rd", newPassword = TestApp.AdminPassword + "x" });
    }

    [Fact]
    public async Task Responses_carry_security_headers_and_errors_do_not_leak_internals()
    {
        var response = await app.Anonymous().GetAsync("/api/competition/active");
        Assert.Equal("nosniff", response.Headers.GetValues("X-Content-Type-Options").Single());
        Assert.Equal("DENY", response.Headers.GetValues("X-Frame-Options").Single());
        Assert.Contains("frame-ancestors 'none'", response.Headers.GetValues("Content-Security-Policy").Single());
        Assert.Equal(HttpStatusCode.NotFound, (await app.Anonymous().GetAsync("/api/does-not-exist")).StatusCode);
    }
}

/// <summary>Upgrading a database that only has the six hand-made tables, with real data in them.</summary>
public class LegacyUpgradeTests(LegacyUpgradeTests.LegacyApp app) : IClassFixture<LegacyUpgradeTests.LegacyApp>
{
    public sealed class LegacyApp : TestApp
    {
        protected override async Task PrepareDatabaseAsync()
        {
            await using var db = NewDbContext();
            // Create exactly what the old system had: the baseline tables and no migration history.
            await db.GetService<IMigrator>().MigrateAsync("Baseline");
            await db.Database.ExecuteSqlRawAsync("DROP TABLE \"__EFMigrationsHistory\"");

            var hasher = new PasswordHasher<AuthUser>();
            foreach (var (id, name) in new[] { (656073L, "9764587864"), (510611L, "9921204201"), (641560L, "7744915362") })
            {
                var hash = hasher.HashPassword(new AuthUser(), "OldPassword#1");
                await db.Database.ExecuteSqlInterpolatedAsync($@"
                    INSERT INTO auth_users (id, user_id, username, email, password_hash, first_name, is_active, email_confirmed, phone_number_confirmed,
                        two_factor_enabled, lockout_enabled, access_failed_count, is_user_deactivated, created_at_utc)
                    VALUES ({Guid.NewGuid()}, {id}, {name}, {name + "@example.com"}, {hash}, 'Legacy', true, true, false, false, true, 0, false, now())");
            }
            await db.Database.ExecuteSqlInterpolatedAsync($@"
                INSERT INTO competition_security (id, user_id, consent_version, terms_accepted, status, violation_type, violation_message, violation_count, disqualified_at_utc, created_at_utc)
                VALUES ({Guid.NewGuid()}, 641560, '1.0', true, 'DISQUALIFIED', 'MULTIPLE_PERSON', 'Multiple persons detected.', 1, now() - interval '1 day', now())");
            var adminHash = new PasswordHasher<AdminUser>().HashPassword(new AdminUser(), "Admin@0765");
            await db.Database.ExecuteSqlInterpolatedAsync($@"
                INSERT INTO admin_users (id, username, password_hash, display_name, is_active, created_at_utc, access_failed_count)
                VALUES ({Guid.NewGuid()}, 'Sachin', {adminHash}, 'Sachin', true, now(), 0)");
            await db.Database.ExecuteSqlRawAsync(@"
                INSERT INTO competition_settings (id, competition_name, start_date_time_utc, end_date_time_utc, is_active, created_at_utc)
                VALUES (gen_random_uuid(), 'HEM Competition 2026', now() - interval '1 hour', now() + interval '1 hour', true, now())");
        }
    }

    [Fact]
    public async Task Existing_tables_are_adopted_and_the_rest_is_added()
    {
        await app.DbAsync(async db =>
        {
            Assert.Equal(2, (await db.Database.GetAppliedMigrationsAsync()).Count());
            Assert.Equal(3, await db.AuthUsers.CountAsync());
            Assert.Equal(6, await db.Games.CountAsync());
            Assert.Equal("HEM Competition 2026", (await db.CompetitionSettings.SingleAsync()).CompetitionName);
        });
    }

    [Fact]
    public async Task Existing_students_sign_in_with_their_old_password_and_can_play()
    {
        var client = await app.LoginStudentAsync("9764587864", "OldPassword#1");
        var dashboard = await client.GetJson("/api/student/dashboard");
        Assert.Equal(656073, dashboard.GetProperty("profile").GetProperty("userId").GetInt64());
        Assert.Equal("Live", dashboard.GetProperty("competition").GetProperty("phase").GetString());
        var attempt = await client.PostJson("/api/games/game24/start");
        Assert.Equal("InProgress", attempt.GetProperty("status").GetString());
    }

    [Fact]
    public async Task Previously_disqualified_student_gets_a_ban_record_with_an_appeal_option()
    {
        var response = await app.Anonymous().PostAsJsonAsync("/api/auth/login", app.LoginBody("7744915362", "OldPassword#1"));
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("BANNED", body.GetProperty("code").GetString());
        Assert.Equal("Multiple persons detected.", body.GetProperty("ban").GetProperty("reason").GetString());
        Assert.False(string.IsNullOrEmpty(body.GetProperty("appealToken").GetString()));
    }

    [Fact]
    public async Task Existing_admin_keeps_access_but_must_replace_the_published_password()
    {
        var client = app.Anonymous();
        var login = await client.PostJson("/api/admin/auth/login", new { username = "Sachin", password = "Admin@0765" });
        Assert.True(login.GetProperty("profile").GetProperty("mustChangePassword").GetBoolean());
        Assert.Equal("Super Admin", login.GetProperty("profile").GetProperty("role").GetString());
        // No new bootstrap account is added to a database that already has admins.
        Assert.Equal(1, await app.DbAsync(db => db.AdminUsers.CountAsync()));
    }
}

/// <summary>A half-present legacy schema must stop the upgrade rather than be guessed at.</summary>
public class BrokenLegacyTests
{
    private sealed class PartialApp : TestApp
    {
        protected override async Task PrepareDatabaseAsync()
        {
            await using var db = NewDbContext();
            await db.Database.ExecuteSqlRawAsync("CREATE TABLE auth_users (id uuid primary key, user_id bigint not null)");
        }
    }

    [Fact]
    public async Task Partial_legacy_schema_refuses_to_start()
    {
        var app = new PartialApp();
        try
        {
            var ex = await Assert.ThrowsAnyAsync<Exception>(() => app.InitializeAsync());
            Assert.Contains("some but not all", ex.ToString());
        }
        finally
        {
            await using var admin = new NpgsqlConnection((Environment.GetEnvironmentVariable("TEST_PG") ?? "Host=localhost;Username=postgres;Password=postgres") + ";Database=postgres;Pooling=false");
            await admin.OpenAsync();
            await using var cmd = new NpgsqlCommand($"DROP DATABASE IF EXISTS \"{app.DatabaseName}\" WITH (FORCE)", admin);
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
