using Microsoft.AspNetCore.Identity;
using PhotoChallenge.DataAccess.Context;

namespace PhotoChallenge.Server.Extensions
{
    public static class DataContextExtensions
    {
        public static void EnsureSeeds(this DataContext context, RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(context, roleManager);
        }

        private static void SeedRoles(DataContext context, RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("User").Result)
            {
                roleManager.CreateAsync(new IdentityRole("User")).Wait();
            }

            if (!roleManager.RoleExistsAsync("Moderator").Result)
            {
                roleManager.CreateAsync(new IdentityRole("Moderator")).Wait();
            }

            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                roleManager.CreateAsync(new IdentityRole("Admin")).Wait();
            }
        }
    }
}
