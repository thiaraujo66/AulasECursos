using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AulasECursos.Infrastructure.Data
{
    public static class IdentitySeeder
    {
        private static readonly string[] Roles = { "Admin", "Instructor", "Student" };

        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            await EnsureRolesExistAsync(roleManager);
            await EnsureAdminUserExistsAsync(userManager, configuration);
        }

        private static async Task EnsureRolesExistAsync(RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in Roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private static async Task EnsureAdminUserExistsAsync(
            UserManager<IdentityUser> userManager,
            IConfiguration configuration)
        {
            // Credenciais do admin vêm de configuração (user-secrets em dev,
            // variável de ambiente em produção) — nunca hardcoded no código.
            var adminEmail = configuration["Seed:AdminEmail"]
                ?? throw new InvalidOperationException("Configuração 'Seed:AdminEmail' não encontrada.");
            var adminPassword = configuration["Seed:AdminPassword"]
                ?? throw new InvalidOperationException("Configuração 'Seed:AdminPassword' não encontrada.");

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser is null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Falha ao criar o usuário admin no seed: {errors}");
                }
            }

            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}
