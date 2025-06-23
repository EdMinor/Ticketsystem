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
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();


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


            /* Für Default Admin/User
             * admin@demo.de admin123!
             * user@demo.de user123!
                User    mo@demo.de  mo123!
                User	eduard@demo.de	eduard123!	
                User	suat@demo.de	suat123!	
                User	ahmad@demo.de	ahmad123!	
             */
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                await Ticketsystem.Models.DbInit.SeedRolesAndUsersAsync(userManager, roleManager);
                
                
                app.Run();
            }
        }
    }
}
