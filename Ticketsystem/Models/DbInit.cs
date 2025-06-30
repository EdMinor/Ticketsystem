using Microsoft.AspNetCore.Identity;


namespace Ticketsystem.Models
{
    public static class DbInit
    {
        public static async Task SeedRolesAndUsersAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Rollen anlegen
            string[] roles = { "Admin", "User", "Developer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Admin-Benutzer
            var adminEmail = "admin@demo.de";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new ApplicationUser { UserName = adminEmail, Email = adminEmail };
                var result = await userManager.CreateAsync(admin, "Admin123!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }

            // Standard-User
            var userEmail = "user@demo.de";
            if (await userManager.FindByEmailAsync(userEmail) == null)
            {
                var user = new ApplicationUser { UserName = userEmail, Email = userEmail };
                var result = await userManager.CreateAsync(user, "User123!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, "User");
            }

            // Mo
            var userMoEmail = "mo@demo.de";
            if (await userManager.FindByEmailAsync(userMoEmail) == null)
            {
                var userMo = new ApplicationUser { UserName = userMoEmail, Email = userMoEmail };
                var result = await userManager.CreateAsync(userMo, "Mo123!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(userMo, "User");
            }
            // Eduard
            var userEduardEmail = "eduard@demo.de";
            if (await userManager.FindByEmailAsync(userEduardEmail) == null)
            {
                var userEduard = new ApplicationUser { UserName = userEduardEmail, Email = userEduardEmail };
                var result = await userManager.CreateAsync(userEduard, "Eduard123!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(userEduard, "User");
            }

            // Suat
            var userSuatEmail = "suat@demo.de";
            if (await userManager.FindByEmailAsync(userSuatEmail) == null)
            {
                var userSuat = new ApplicationUser { UserName = userSuatEmail, Email = userSuatEmail };
                var result = await userManager.CreateAsync(userSuat, "Suat123!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(userSuat, "User");
            }

            // Ahmad
            var userAhmadEmail = "ahmad@demo.de";
            if (await userManager.FindByEmailAsync(userAhmadEmail) == null)
            {
                var userAhmad = new ApplicationUser { UserName = userAhmadEmail, Email = userAhmadEmail };
                var result = await userManager.CreateAsync(userAhmad, "Ahmad123!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(userAhmad, "User");
            }
        }

        public static async Task SeedTicketsAsync(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            // Beenden, wenn schon Tickets da sind
            if (context.Tickets.Any())
            {
                return;
            }

            var users = userManager.Users.ToList();
            var random = new Random();
            var generalCategory = context.Categories.FirstOrDefault(c => c.Titel == "Sonstiges");
            var generalCategoryId = generalCategory?.Id;

            var ticketTitles = new List<string> { "Drucker funktioniert nicht", "Passwort zurücksetzen", "Software-Installation anfordern", "Netzwerkproblem", "Monitor defekt", "Maus reagiert nicht", "Anwendungsfehler", "Zugriffsrechte benötigt", "Neuer Account für Mitarbeiter", "Server nicht erreichbar" };
            var statuses = new[] { "Offen", "In Bearbeitung", "Geschlossen" };
            var priorities = new[] { "Niedrig", "Mittel", "Hoch" };

            foreach (var user in users)
            {
                var numberOfTickets = random.Next(5, 11); // 5 bis 10 Tickets pro User
                for (int i = 0; i < numberOfTickets; i++)
                {
                    var ticket = new Ticket
                    {
                        Title = ticketTitles[random.Next(ticketTitles.Count)],
                        Description = "Dies ist eine automatisch generierte Beschreibung für das Ticket.",
                        Status = statuses[random.Next(statuses.Length)],
                        Priority = priorities[random.Next(priorities.Length)],
                        CategoryId = generalCategoryId,
                        CreatorId = user.Id,
                        CreatedAt = DateTime.Now.AddDays(-random.Next(0, 15)) // Datum der letzten 14 Tage
                    };
                    context.Tickets.Add(ticket);
                }
            }
            await context.SaveChangesAsync();
        }

        public static async Task SeedCategoriesAsync(AppDbContext context)
        {
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Titel = "Verwaltung", Beschreibung = "Verwaltungsprojekte", Startdatum = DateTime.Now },
                    new Category { Titel = "Schulungsräume", Beschreibung = "Schulungsräume", Startdatum = DateTime.Now },
                    new Category { Titel = "Sonstiges", Beschreibung = "Sonstiges", Startdatum = DateTime.Now }
                };
                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
            }
        }
    }
}