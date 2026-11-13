using HemCompetition.API.Domain;
using HemCompetition.API.Infrastructure;
using HemCompetition.API.Security;
using HemCompetition.API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Options;

namespace HemCompetition.API.Data;

/// <summary>
/// Brings a database up to date at start-up.
///
/// The six original tables were created by hand before the project used migrations. When they are
/// already present, the "Baseline" migration (which describes exactly those tables) is recorded as
/// applied instead of being run, so existing data is never recreated or dropped. Everything after
/// the baseline is additive.
/// </summary>
public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services, ILogger logger)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await AdoptExistingSchemaAsync(db, logger);
        await db.Database.MigrateAsync();
        await SeedAsync(db, scope.ServiceProvider.GetRequiredService<IOptions<BootstrapOptions>>().Value, logger);
    }

    private static async Task AdoptExistingSchemaAsync(AppDbContext db, ILogger logger)
    {
        if (!await db.Database.CanConnectAsync())
            throw new InvalidOperationException("Cannot connect to PostgreSQL. Check ConnectionStrings:Default.");
        var existing = await db.Database.SqlQueryRaw<string>(
                "SELECT table_name AS \"Value\" FROM information_schema.tables WHERE table_schema = current_schema()")
            .ToListAsync();
        if (existing.Contains("__EFMigrationsHistory")) return; // already under migration control

        string[] legacy = ["auth_users", "auth_sessions", "competition_security", "competition_settings", "admin_users", "ExceptionLogs"];
        var present = legacy.Where(existing.Contains).ToList();
        if (present.Count == 0) return; // brand-new database: run every migration normally

        if (present.Count != legacy.Length)
            throw new InvalidOperationException(
                "The database contains some but not all of the original tables (" + string.Join(", ", present) +
                "). Restore the missing tables or start from an empty database; refusing to guess.");

        var duplicates = await db.Database.SqlQueryRaw<int>(
            "SELECT COUNT(*)::int AS \"Value\" FROM (SELECT user_id FROM auth_users GROUP BY user_id HAVING COUNT(*) > 1) d").FirstAsync();
        if (duplicates > 0)
            throw new InvalidOperationException(
                $"auth_users has {duplicates} duplicated user_id value(s). Each student must have exactly one row before upgrading.");

        var baseline = db.GetService<IMigrationsAssembly>().Migrations.Keys.OrderBy(k => k).First();
        var history = db.GetService<IHistoryRepository>();
        await db.Database.ExecuteSqlRawAsync(history.GetCreateIfNotExistsScript());
        await db.Database.ExecuteSqlRawAsync(history.GetInsertScript(new HistoryRow(baseline, ProductInfo.GetVersion())));
        logger.LogInformation("Existing tables detected; recorded {Baseline} as already applied.", baseline);
    }

    private static async Task SeedAsync(AppDbContext db, BootstrapOptions bootstrap, ILogger logger)
    {
        // --- roles ---
        var roles = await db.AdminRoles.Include(r => r.Permissions).ToListAsync();
        foreach (var (name, description, permissions) in Permissions.DefaultRoles)
        {
            if (roles.Any(r => r.Name == name)) continue;
            var role = new AdminRole
            {
                Name = name, Description = description, IsSystem = true,
                Permissions = permissions.Select(p => new AdminRolePermission { Permission = p }).ToList()
            };
            db.AdminRoles.Add(role);
            roles.Add(role);
        }
        await db.SaveChangesAsync();
        var superAdmin = roles.First(r => r.Name == Permissions.SuperAdminRole);

        // Admins that pre-date roles had full access, so they keep it.
        await db.AdminUsers.Where(a => a.RoleId == null).ExecuteUpdateAsync(u => u.SetProperty(a => a.RoleId, superAdmin.Id));

        if (!await db.AdminUsers.AnyAsync())
        {
            if (string.IsNullOrWhiteSpace(bootstrap.AdminUsername) || string.IsNullOrWhiteSpace(bootstrap.AdminPassword))
                logger.LogWarning("No admin accounts exist. Set Bootstrap:AdminUsername and Bootstrap:AdminPassword to create the first one.");
            else
            {
                var admin = new AdminUser
                {
                    Id = Guid.NewGuid(), Username = bootstrap.AdminUsername.Trim(), DisplayName = "Administrator",
                    IsActive = true, CreatedAtUtc = DateTime.UtcNow, RoleId = superAdmin.Id, MustChangePassword = true
                };
                admin.PasswordHash = new PasswordHasher<AdminUser>().HashPassword(admin, bootstrap.AdminPassword);
                db.AdminUsers.Add(admin);
                logger.LogInformation("Created the first admin account '{Username}'. A password change is required at first sign-in.", admin.Username);
            }
        }

        // --- sections and games ---
        if (!await db.GameCategories.AnyAsync())
        {
            db.GameCategories.AddRange(
                new GameCategory
                {
                    Key = "number", Name = "Number Game", SortOrder = 1, Description = "Logic and arithmetic puzzles.",
                    Games =
                    [
                        new Game { Key = "game24", Name = "Game 24", EngineKey = "game24", SortOrder = 1, Description = "Use all four numbers once with + - x / to make 24." },
                        new Game { Key = "futoshiki", Name = "Futoshiki", EngineKey = "futoshiki", SortOrder = 2, Description = "Fill the grid so no number repeats in a row or column and every < and > sign holds." },
                        new Game { Key = "kenken", Name = "KenKen", EngineKey = "kenken", SortOrder = 3, Description = "Fill the grid so each coloured cage produces its target number." }
                    ]
                },
                new GameCategory { Key = "quiz", Name = "Mega Quiz", SortOrder = 2, Description = "Multiple-choice quizzes." },
                new GameCategory
                {
                    Key = "word", Name = "Word Game", SortOrder = 3, Description = "Vocabulary puzzles.",
                    Games =
                    [
                        new Game { Key = "scramble", Name = "Scramble", EngineKey = "scramble", SortOrder = 1, Description = "Unscramble each word, then use the highlighted letters to form the final word." },
                        new Game { Key = "puzzle", Name = "Puzzle", EngineKey = "puzzle", SortOrder = 2, Description = "Find every listed word in the letter grid." },
                        new Game { Key = "word", Name = "Word Challenge", EngineKey = "word", SortOrder = 3, Description = "Answer each clue with a word of the stated length." }
                    ]
                });
        }

        // --- Mega Quiz 1-4 (unpublished until questions are added) ---
        if (!await db.QuizCategories.AnyAsync() && !await db.Quizzes.AnyAsync())
        {
            var now = DateTime.UtcNow;
            for (var i = 1; i <= 4; i++)
            {
                var bank = new QuizCategory { Name = $"Mega Quiz {i} question bank", CreatedAtUtc = now };
                db.Quizzes.Add(new Quiz
                {
                    Title = $"HEM Mega Quiz {i}", Category = bank, SortOrder = i, CreatedAtUtc = now,
                    QuestionCount = 10, DurationMinutes = 10, SelectionMode = QuizSelectionMode.Random
                });
            }
        }

        // --- violation policy ---
        var knownRules = await db.ViolationRules.Select(r => r.Code).ToListAsync();
        db.ViolationRules.AddRange(ProctoringService.DefaultRules().Where(r => !knownRules.Contains(r.Code)));

        // --- starter vocabulary ---
        if (bootstrap.SeedWordBank && !await db.WordBank.AnyAsync())
            db.WordBank.AddRange(StarterWords.All.Select(w => new WordBankEntry
            {
                Word = w.Word, Clue = w.Clue, Topic = w.Topic, CreatedAtUtc = DateTime.UtcNow
            }));

        await db.SaveChangesAsync();
    }
}
