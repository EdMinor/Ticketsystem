using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Ticketsystem.Models;

namespace Ticketsystem
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // Datenbank-Verbindung 
            builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("TicketSystemDatenbankVerbindung")));



            // Identity 
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Account/AccessDenied";
});


            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // UseAuthentication() aktiviert... REIHENFOLGE WICHTIG!!!
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");


            /* F�r Default Admin/User
             * admin@demo.de admin123!
             * user@demo.de user123!
                User    mo@demo.de  Mo123!
                User	eduard@demo.de	Eduard123!	
                User	suat@demo.de	Suat123!	
                User	ahmad@demo.de	Ahmad123!	
             */
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var context = services.GetRequiredService<AppDbContext>();

                // DB automatisch migrieren (erstellt DB und Tabellen wenn nicht vorhanden)
                await context.Database.MigrateAsync();

                await DbInit.SeedCategoriesAsync(context);
                await DbInit.SeedRolesAndUsersAsync(userManager, roleManager);
                await DbInit.SeedTicketsAsync(context, userManager);
            }

            app.Run();
        }

    }
    
}
