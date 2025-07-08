using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ticketsystem.Models;
using System.Threading.Tasks;
using System.Linq;
using Ticketsystem.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Ticketsystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var tickets = await _context.Tickets.ToListAsync();
            var users = await _userManager.Users.ToListAsync();
            ViewBag.CategoryCount = await _context.Categories.CountAsync();

            // Получить пользователей с ролью Developer
            var developerUsers = new List<string>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Developer"))
                {
                    var name = !string.IsNullOrWhiteSpace(user.FullName) ? user.FullName : user.UserName;
                    if (name != null)
                    {
                        developerUsers.Add(name);
                    }
                }
            }

            var model = new AdminTicketPanelViewModel
            {
                OpenTicketsCount = tickets.Count(t => t.Status == "Offen"),
                InProgressTicketsCount = tickets.Count(t => t.Status == "In Bearbeitung"),
                ClosedTicketsCount = tickets.Count(t => t.Status == "Geschlossen"),
                OverdueTicketsCount = tickets.Count(t => t.Status != "Geschlossen" && t.CreatedAt < DateTime.Now.AddDays(-3)),
                UserNames = users.Select(u => u.UserName).OfType<string>().ToList(),
                DeveloperUsers = developerUsers
            };

            return View(model);
        }
    }
} 