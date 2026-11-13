using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using HemCompetition.API.Data;
using HemCompetition.API.Diagnostics;
using HemCompetition.API.Games;
using HemCompetition.API.Infrastructure;
using HemCompetition.API.Middleware;
using HemCompetition.API.Security;
using HemCompetition.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// ---------------------------------------------------------------------------------------------
// Configuration. Secrets come from environment variables or user-secrets, never from the repo.
// ---------------------------------------------------------------------------------------------
builder.Services.Configure<JwtOptions>(config.GetSection("Jwt"));
builder.Services.Configure<HeartbeatOptions>(config.GetSection("SessionSettings"));
builder.Services.Configure<StorageOptions>(config.GetSection("Storage"));
builder.Services.Configure<BootstrapOptions>(config.GetSection("Bootstrap"));

var jwt = config.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
if (Encoding.UTF8.GetByteCount(jwt.Key) < 32)
    throw new InvalidOperationException("Jwt:Key must be set to a random secret of at least 32 bytes (environment variable Jwt__Key).");

var connectionString = config.GetConnectionString("Default")
    ?? throw new InvalidOperationException("ConnectionStrings:Default (PostgreSQL) is not configured.");

// ---------------------------------------------------------------------------------------------
// PostgreSQL - the only database.
// ---------------------------------------------------------------------------------------------
builder.Services.AddSingleton<SqlMetricsInterceptor>();
builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    options.UseNpgsql(connectionString);
    options.AddInterceptors(sp.GetRequiredService<SqlMetricsInterceptor>());
});

// ---------------------------------------------------------------------------------------------
// Authentication and authorisation
// ---------------------------------------------------------------------------------------------
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.MapInboundClaims = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
        ValidateIssuer = true,
        ValidIssuer = jwt.Issuer,
        ValidateAudience = true,
        ValidAudience = jwt.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(30),
        NameClaimType = ClaimTypes.Name
    };
});

builder.Services.AddAuthorization(options =>
{
    // A token is only good for the kind of user it was issued to.
    options.AddPolicy(Policies.Student, p => p.RequireAuthenticatedUser().RequireClaim(TokenTypes.Claim, TokenTypes.Student));
    options.AddPolicy(Policies.Admin, p => p.RequireAuthenticatedUser().RequireClaim(TokenTypes.Claim, TokenTypes.Admin));
    options.AddPolicy(Policies.StudentOrAppeal, p => p.RequireAuthenticatedUser().RequireClaim(TokenTypes.Claim, TokenTypes.Student, TokenTypes.Appeal));
});

// ---------------------------------------------------------------------------------------------
// Rate limiting
// ---------------------------------------------------------------------------------------------
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, ct) =>
        await context.HttpContext.Response.WriteAsJsonAsync(new { message = "Too many requests. Please wait a moment and try again.", code = "RATE_LIMITED" }, ct);

    static string Ip(HttpContext c) => c.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    static string Who(HttpContext c) => c.User.FindFirst("UserId")?.Value ?? c.User.FindFirst("AdminId")?.Value ?? Ip(c);

    // Whole schools sign in from one address, so the per-address limit is generous;
    // guessing a single account is stopped by the per-account lockout instead.
    options.AddPolicy("login", c => RateLimitPartition.GetSlidingWindowLimiter(Ip(c),
        _ => new SlidingWindowRateLimiterOptions { PermitLimit = 120, Window = TimeSpan.FromMinutes(1), SegmentsPerWindow = 6 }));
    options.AddPolicy("refresh", c => RateLimitPartition.GetSlidingWindowLimiter(Ip(c),
        _ => new SlidingWindowRateLimiterOptions { PermitLimit = 600, Window = TimeSpan.FromMinutes(1), SegmentsPerWindow = 6 }));
    options.AddPolicy("proctoring", c => RateLimitPartition.GetSlidingWindowLimiter(Who(c),
        _ => new SlidingWindowRateLimiterOptions { PermitLimit = 90, Window = TimeSpan.FromMinutes(1), SegmentsPerWindow = 6 }));
    options.AddPolicy("appeal", c => RateLimitPartition.GetFixedWindowLimiter(Who(c),
        _ => new FixedWindowRateLimiterOptions { PermitLimit = 5, Window = TimeSpan.FromHours(1) }));
    // Ceiling for everything else, per signed-in user (or per address when anonymous).
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(c =>
        c.Request.Path.StartsWithSegments("/api")
            ? RateLimitPartition.GetSlidingWindowLimiter(c.User.Identity?.IsAuthenticated == true ? Who(c) : "ip:" + Ip(c),
                _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = c.User.Identity?.IsAuthenticated == true ? 300 : 1200,
                    Window = TimeSpan.FromMinutes(1), SegmentsPerWindow = 6
                })
            : RateLimitPartition.GetNoLimiter("static"));
});

// ---------------------------------------------------------------------------------------------
// Application services
// ---------------------------------------------------------------------------------------------
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<SettingsService>();
builder.Services.AddScoped<AuditService>();
builder.Services.AddScoped<AdminAccessService>();
builder.Services.AddScoped<CompetitionService>();
builder.Services.AddScoped<BanService>();
builder.Services.AddScoped<GameAttemptService>();
builder.Services.AddScoped<QuizAttemptService>();
builder.Services.AddScoped<ProctoringService>();
builder.Services.AddScoped<LeaderboardService>();
builder.Services.AddScoped<QuestionImportService>();
builder.Services.AddSingleton<IFileStorage, LocalFileStorage>();

// Game engines: register a new IGameEngine here and add a row to `games` to add a game.
builder.Services.AddSingleton<IGameEngine, Game24Engine>();
builder.Services.AddSingleton<IGameEngine, KenKenEngine>();
builder.Services.AddSingleton<IGameEngine, FutoshikiEngine>();
builder.Services.AddSingleton<IGameEngine, ScrambleEngine>();
builder.Services.AddSingleton<IGameEngine, WordSearchEngine>();
builder.Services.AddSingleton<IGameEngine, WordChallengeEngine>();
builder.Services.AddSingleton<GameEngineRegistry>();

// API telemetry shown on the admin diagnostics pages
builder.Services.AddSingleton<IApiMetricsService, ApiMetricsService>();
builder.Services.AddSingleton<ISqlDiagnosticsService, SqlDiagnosticsService>();
builder.Services.AddSingleton<IApiDiagnosticsService, ApiDiagnosticsService>();

if (config.GetValue("Maintenance:Enabled", true))
    builder.Services.AddHostedService<MaintenanceService>();

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()))
    .ConfigureApiBehaviorOptions(options =>
{
    // One error shape everywhere: { message, code, errors }
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState.Where(e => e.Value?.Errors.Count > 0)
            .ToDictionary(e => e.Key, e => e.Value!.Errors.Select(x => x.ErrorMessage).ToArray());
        var first = errors.Values.SelectMany(v => v).FirstOrDefault(m => !string.IsNullOrWhiteSpace(m)) ?? "The request is not valid.";
        return new BadRequestObjectResult(new { message = first, code = "VALIDATION", errors });
    };
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (config.GetValue("ReverseProxy:Enabled", false))
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        // Only trust the proxy on this machine unless told otherwise.
        foreach (var proxy in config.GetSection("ReverseProxy:KnownProxies").Get<string[]>() ?? [])
            options.KnownProxies.Add(System.Net.IPAddress.Parse(proxy));
    });

var app = builder.Build();

await DatabaseInitializer.InitializeAsync(app.Services, app.Logger);

// ---------------------------------------------------------------------------------------------
// Pipeline
// ---------------------------------------------------------------------------------------------
if (config.GetValue("ReverseProxy:Enabled", false)) app.UseForwardedHeaders();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseMiddleware<SecurityHeadersMiddleware>();

var contentTypes = new FileExtensionContentTypeProvider();
contentTypes.Mappings[".tflite"] = "application/octet-stream";
contentTypes.Mappings[".wasm"] = "application/wasm";
app.UseStaticFiles(new StaticFileOptions { ContentTypeProvider = contentTypes });

app.UseRouting();
app.UseMiddleware<ApiMetricsMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAuthentication();
app.UseRateLimiter();
app.UseMiddleware<SessionValidationMiddleware>();
app.UseAuthorization();

app.MapControllers();
// Unknown /api routes are a 404, not the single-page app.
app.Map("/api/{**rest}", () => Results.NotFound(new { message = "Not found.", code = "NOT_FOUND" }));
app.MapFallbackToFile("index.html");

app.Run();

public partial class Program;
