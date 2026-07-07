using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RegulatoryComplianceApplication.Core.Entities;

namespace RegulatoryComplianceApplication.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();


            var exists = await context.Users.AnyAsync(u => u.Email == "newadmin@test.com");
         

            if (!exists)
            {
                

                var admin = new User
                {
                    FullName = "Majed AlGhamdi",
                    Email = "newadmin@test.com",
                    RoleId = 1,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var hasher = new PasswordHasher<User>();
                admin.PasswordHash = hasher.HashPassword(admin, "Admin123!");

                context.Users.Add(admin);

                await context.SaveChangesAsync();

            
            }
        }
    }
}