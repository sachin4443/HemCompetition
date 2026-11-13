using HemCompetition.API.Data;
using HemCompetition.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HemCompetition.API.Services
{
    public static class AdminUserSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var context =
                scope.ServiceProvider
                    .GetRequiredService<AuthDbContext>();

            // Make sure table exists.
            // IMPORTANT:
            // No EF migration is executed here.
            // Table must already be created manually in PostgreSQL.

            var passwordHasher =
                new PasswordHasher<AdminUser>();

            var admins = new[]
            {
                new
                {
                    Username = "Admin",
                    Password = "Admin@0765",
                    DisplayName = "Administrator"
                },
                new
                {
                    Username = "Sachin",
                    Password = "Admin@0765",
                    DisplayName = "Sachin"
                },
                new
                {
                    Username = "Rajan",
                    Password = "Admin@0765",
                    DisplayName = "Rajan"
                }
            };

            foreach (var item in admins)
            {
                var username = item.Username.Trim();

                var existingAdmin =
                    await context.AdminUsers
                        .FirstOrDefaultAsync(x =>
                            x.Username == username);

                if (existingAdmin != null)
                {
                    continue;
                }

                var admin = new AdminUser
                {
                    Id = Guid.NewGuid(),

                    Username = username,

                    DisplayName = item.DisplayName,

                    IsActive = true,

                    CreatedAtUtc = DateTime.UtcNow
                };

                admin.PasswordHash =
                    passwordHasher.HashPassword(
                        admin,
                        item.Password);

                context.AdminUsers.Add(admin);
            }

            await context.SaveChangesAsync();
        }
    }
}