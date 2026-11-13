using HemCompetition.API.Data;
using HemCompetition.API.Diagnostics;
using HemCompetition.API.Middleware;
using HemCompetition.API.Models;
using HemCompetition.API.Services;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using System.Text;

var builder = WebApplication.CreateBuilder(args);


// =====================================================
// DATABASE
// =====================================================

// -----------------------------------------------------
// MAIN COMPETITION DATABASE - SQL SERVER
// -----------------------------------------------------
builder.Services.AddDbContext<Hemvirtues2627Context>(
    (serviceProvider, options) =>
    {
        options.UseSqlServer(
            builder.Configuration.GetConnectionString(
                "DefaultConnection"));

        // SQL diagnostics / query metrics
        options.AddInterceptors(
            serviceProvider.GetRequiredService<
                SqlMetricsInterceptor>());
    });


// -----------------------------------------------------
// AUTH DATABASE - POSTGRESQL
// -----------------------------------------------------
builder.Services.AddDbContext<AuthDbContext>(
    (serviceProvider, options) =>
    {
        options.UseNpgsql(
            builder.Configuration.GetConnectionString(
                "AuthConnection"));

        // SQL diagnostics / query metrics
        // DbCommandInterceptor works with PostgreSQL also.
        options.AddInterceptors(
            serviceProvider.GetRequiredService<
                SqlMetricsInterceptor>());
    });


// =====================================================
// JWT
// =====================================================

var jwtSettings =
    builder.Configuration.GetSection("Jwt");

var jwtKey =
    jwtSettings["Key"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException(
        "Jwt:Key is missing from configuration.");
}

var key =
    Encoding.UTF8.GetBytes(jwtKey);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,

                IssuerSigningKey =
                    new SymmetricSecurityKey(key),

                ValidateIssuer = true,

                ValidIssuer =
                    jwtSettings["Issuer"],

                ValidateAudience = true,

                ValidAudience =
                    jwtSettings["Audience"],

                ValidateLifetime = true,

                ClockSkew =
                    TimeSpan.Zero
            };
    });


// =====================================================
// AUTHORIZATION
// =====================================================

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        "AdminOnly",
        policy =>
        {
            policy.RequireAuthenticatedUser();

            // HARD-CODED ADMIN ROLE
            policy.RequireClaim(
                "AdminRole",
                "Admin");
        });
});


// =====================================================
// SERVICES
// =====================================================

// -----------------------------------------------------
// EXISTING SERVICES
// -----------------------------------------------------

builder.Services.AddScoped<
    IAuthSessionService,
    AuthSessionService>();

builder.Services.AddScoped<
    IFutoshikiService,
    FutoshikiService>();

builder.Services.AddScoped<
    IKenkenService,
    KenkenService>();


// -----------------------------------------------------
// CACHE
// -----------------------------------------------------

builder.Services.AddMemoryCache();


// -----------------------------------------------------
// API METRICS
// -----------------------------------------------------

builder.Services.AddSingleton<
    IApiMetricsService,
    ApiMetricsService>();


// -----------------------------------------------------
// SQL DIAGNOSTICS
// -----------------------------------------------------

builder.Services.AddSingleton<
    ISqlDiagnosticsService,
    SqlDiagnosticsService>();


// -----------------------------------------------------
// API DIAGNOSTICS
// -----------------------------------------------------

builder.Services.AddSingleton<
    IApiDiagnosticsService,
    ApiDiagnosticsService>();


// -----------------------------------------------------
// SQL COMMAND INTERCEPTOR
// -----------------------------------------------------
//
// IMPORTANT:
// Same interceptor is attached to BOTH DbContexts
// above.
// -----------------------------------------------------

builder.Services.AddSingleton<
    SqlMetricsInterceptor>();


// =====================================================
// CONTROLLERS
// =====================================================

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();


// =====================================================
// SWAGGER
// =====================================================

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Description =
                "Enter JWT token as: Bearer {token}",

            Name =
                "Authorization",

            In =
                ParameterLocation.Header,

            Type =
                SecuritySchemeType.Http,

            Scheme =
                "Bearer",

            BearerFormat =
                "JWT"
        });

    c.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference =
                        new OpenApiReference
                        {
                            Type =
                                ReferenceType.SecurityScheme,

                            Id =
                                "Bearer"
                        }
                },

                Array.Empty<string>()
            }
        });
});


var app = builder.Build();


// =====================================================
// DEVELOPMENT
// =====================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}


// =====================================================
// STATIC FILES
// =====================================================

app.UseHttpsRedirection();

var provider =
    new FileExtensionContentTypeProvider();

provider.Mappings[".tflite"] =
    "application/octet-stream";

// Serve React static files from wwwroot on Publish
app.UseStaticFiles(
    new StaticFileOptions
    {
        ContentTypeProvider =
            provider
    });


// =====================================================
// ROUTING
// =====================================================

app.UseRouting();


// =====================================================
// API METRICS MIDDLEWARE
// =====================================================
//
// Must run before Authentication so request timing,
// path and trace information can be captured for the
// complete request pipeline.
// =====================================================

app.UseMiddleware<ApiMetricsMiddleware>();


// =====================================================
// AUTHENTICATION
// =====================================================

app.UseAuthentication();


// =====================================================
// SESSION MIDDLEWARE
// =====================================================

app.UseMiddleware<SessionValidationMiddleware>();

app.UseMiddleware<GlobalExceptionMiddleware>();
// =====================================================
// AUTHORIZATION
// =====================================================

app.UseAuthorization();


// =====================================================
// API CONTROLLERS
// =====================================================

app.MapControllers();


// =====================================================
// ADMIN SEEDER
// =====================================================

await AdminUserSeeder.SeedAsync(
    app.Services);


// =====================================================
// REACT SPA FALLBACK
// =====================================================

app.MapFallbackToFile(
    "index.html");


// =====================================================
// RUN
// =====================================================

app.Run();