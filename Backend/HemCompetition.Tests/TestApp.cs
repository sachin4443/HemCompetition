using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using HemCompetition.API.Data;
using HemCompetition.API.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace HemCompetition.Tests;

/// <summary>
/// The real application on a throw-away PostgreSQL database. Set TEST_PG to point at another
/// server (default: Host=localhost;Username=postgres;Password=postgres).
/// </summary>
public class TestApp : IAsyncLifetime
{
    public const string AdminPassword = "Admin-Test-Pass-2026";
    public const string StudentPassword = "Student#Pass1";

    private static readonly string Server =
        Environment.GetEnvironmentVariable("TEST_PG") ?? "Host=localhost;Username=postgres;Password=postgres";

    public string DatabaseName { get; } = "hc_test_" + Guid.NewGuid().ToString("N")[..12];
    public string ConnectionString => $"{Server};Database={DatabaseName};Pooling=false";
    public string StorageRoot { get; } = Path.Combine(Path.GetTempPath(), "hc_test_storage_" + Guid.NewGuid().ToString("N"));
    public WebApplicationFactory<Program> Factory { get; private set; } = null!;

    /// <summary>Runs before the application starts, against an empty database.</summary>
    protected virtual Task PrepareDatabaseAsync() => Task.CompletedTask;

    public async Task InitializeAsync()
    {
        await using (var admin = new NpgsqlConnection($"{Server};Database=postgres;Pooling=false"))
        {
            await admin.OpenAsync();
            await using var cmd = new NpgsqlCommand($"CREATE DATABASE \"{DatabaseName}\"", admin);
            await cmd.ExecuteNonQueryAsync();
        }
        await PrepareDatabaseAsync();

        Factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            // UseSetting reaches configuration before Program.cs reads it (ConfigureAppConfiguration would be too late).
            builder.UseSetting("ConnectionStrings:Default", ConnectionString);
            builder.UseSetting("Jwt:Key", "integration-test-signing-key-0123456789abcdef");
            builder.UseSetting("Bootstrap:AdminUsername", "root");
            builder.UseSetting("Bootstrap:AdminPassword", AdminPassword);
            builder.UseSetting("Maintenance:Enabled", "false");
            builder.UseSetting("Storage:Root", StorageRoot);
        });
        _ = Factory.Server; // start the host (runs migrations and seeding)
    }

    public async Task DisposeAsync()
    {
        await Factory.DisposeAsync();
        await using var admin = new NpgsqlConnection($"{Server};Database=postgres;Pooling=false");
        await admin.OpenAsync();
        await using var cmd = new NpgsqlCommand($"DROP DATABASE IF EXISTS \"{DatabaseName}\" WITH (FORCE)", admin);
        await cmd.ExecuteNonQueryAsync();
        if (Directory.Exists(StorageRoot)) Directory.Delete(StorageRoot, true);
    }

    public AppDbContext NewDbContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(ConnectionString).Options);

    public async Task<T> DbAsync<T>(Func<AppDbContext, Task<T>> work)
    {
        await using var db = NewDbContext();
        return await work(db);
    }

    public Task DbAsync(Func<AppDbContext, Task> work) => DbAsync(async db => { await work(db); return 0; });

    public HttpClient Anonymous()
    {
        var client = Factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
        return client;
    }

    private static long _nextStudent = 900000;

    public async Task<long> CreateStudentAsync(string? username = null)
    {
        var id = Interlocked.Increment(ref _nextStudent);
        var user = new AuthUser
        {
            Id = Guid.NewGuid(), UserId = id, Username = username ?? $"student{id}", FirstName = "Test", LastName = $"Student {id}",
            IsActive = true, CreatedAtUtc = DateTime.UtcNow
        };
        user.PasswordHash = new PasswordHasher<AuthUser>().HashPassword(user, StudentPassword);
        await DbAsync(async db => { db.AuthUsers.Add(user); await db.SaveChangesAsync(); });
        return id;
    }

    /// <summary>Creates a student and returns a signed-in client.</summary>
    public async Task<(HttpClient Client, long StudentId)> StudentAsync()
    {
        var id = await CreateStudentAsync();
        return (await LoginStudentAsync($"student{id}"), id);
    }

    public async Task<HttpClient> LoginStudentAsync(string username, string password = StudentPassword)
    {
        var client = Anonymous();
        var response = await client.PostAsJsonAsync("/api/auth/login", new { usernameOrEmail = username, password });
        Assert.True(response.IsSuccessStatusCode, await response.Content.ReadAsStringAsync());
        var token = (await response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("token").GetString();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    /// <summary>Creates an admin with the given role (password already changed) and returns a signed-in client.</summary>
    public async Task<HttpClient> AdminAsync(string role = "Super Admin")
    {
        var username = "adm_" + Guid.NewGuid().ToString("N")[..8];
        await DbAsync(async db =>
        {
            var roleId = await db.AdminRoles.Where(r => r.Name == role).Select(r => r.Id).FirstAsync();
            var admin = new AdminUser { Id = Guid.NewGuid(), Username = username, IsActive = true, CreatedAtUtc = DateTime.UtcNow, RoleId = roleId };
            admin.PasswordHash = new PasswordHasher<AdminUser>().HashPassword(admin, AdminPassword);
            db.AdminUsers.Add(admin);
            await db.SaveChangesAsync();
        });
        var client = Anonymous();
        var response = await client.PostAsJsonAsync("/api/admin/auth/login", new { username, password = AdminPassword });
        Assert.True(response.IsSuccessStatusCode, await response.Content.ReadAsStringAsync());
        var token = (await response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("token").GetString();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    /// <summary>Creates and activates a competition that is running now.</summary>
    public Task<Guid> LiveCompetitionAsync(bool showResults = false, bool showLeaderboard = false, int endsInMinutes = 120) => DbAsync(async db =>
    {
        await db.CompetitionSettings.ExecuteUpdateAsync(u => u.SetProperty(c => c.IsActive, false));
        var c = new CompetitionSetting
        {
            Id = Guid.NewGuid(), CompetitionName = "Test competition", StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-5),
            EndDateTimeUtc = DateTime.UtcNow.AddMinutes(endsInMinutes), IsActive = true, CreatedAtUtc = DateTime.UtcNow,
            ShowResultsToStudents = showResults, ShowLeaderboard = showLeaderboard, ProctoringRequired = true
        };
        db.CompetitionSettings.Add(c);
        await db.SaveChangesAsync();
        return c.Id;
    });
}

public static class HttpExtensions
{
    public static async Task<JsonElement> Json(this HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode, $"{(int)response.StatusCode}: {text}");
        return JsonSerializer.Deserialize<JsonElement>(text);
    }

    public static async Task<JsonElement> GetJson(this HttpClient client, string url) => await (await client.GetAsync(url)).Json();
    public static async Task<JsonElement> PostJson(this HttpClient client, string url, object? body = null) =>
        await (await client.PostAsJsonAsync(url, body ?? new { })).Json();
    public static async Task<JsonElement> PutJson(this HttpClient client, string url, object body) =>
        await (await client.PutAsJsonAsync(url, body)).Json();
    public static async Task<string> Code(this HttpResponseMessage response) =>
        (await response.Content.ReadFromJsonAsync<JsonElement>()).TryGetProperty("code", out var c) ? c.GetString() ?? "" : "";
}
