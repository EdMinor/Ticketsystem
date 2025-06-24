using Microsoft.AspNetCore.Identity;


namespace Ticketsystem.Models
{
    public static class DbInit
    {
        public static async Task SeedRolesAndUsersAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Rollen anlegen
            string[] roles = { "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Admin-Benutzer
            var adminEmail = "admin@demo.de";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new IdentityUser { UserName = adminEmail, Email = adminEmail };
                var result = await userManager.CreateAsync(admin, "Admin123!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }

            // Standard-User
            var userEmail = "user@demo.de";
            if (await userManager.FindByEmailAsync(userEmail) == null)
            {
                var user = new IdentityUser { UserName = userEmail, Email = userEmail };
                var result = await userManager.CreateAsync(user, "User123!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, "User");
            }

            // Mo
            var userMoEmail = "mo@demo.de";
            if (await userManager.FindByEmailAsync(userMoEmail) == null)
            {
                var userMo = new IdentityUser { UserName = userMoEmail, Email = userMoEmail };
                var result = await userManager.CreateAsync(userMo, "Mo123!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(userMo, "User");
            }
            // Eduard
            var userEduardEmail = "eduard@demo.de";
            if (await userManager.FindByEmailAsync(userEduardEmail) == null)
            {
                var userEduard = new IdentityUser { UserName = userEduardEmail, Email = userEduardEmail };
                var result = await userManager.CreateAsync(userEduard, "Eduard123!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(userEduard, "User");
            }

            // Suat
            var userSuatEmail = "suat@demo.de";
            if (await userManager.FindByEmailAsync(userSuatEmail) == null)
            {
                var userSuat = new IdentityUser { UserName = userSuatEmail, Email = userSuatEmail };
                var result = await userManager.CreateAsync(userSuat, "Suat123!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(userSuat, "User");
            }

            // Ahmad
            var userAhmadEmail = "ahmad@demo.de";
            if (await userManager.FindByEmailAsync(userAhmadEmail) == null)
            {
                var userAhmad = new IdentityUser { UserName = userAhmadEmail, Email = userAhmadEmail };
                var result = await userManager.CreateAsync(userAhmad, "Ahmad123!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(userAhmad, "User");
            }
        }
    }
}