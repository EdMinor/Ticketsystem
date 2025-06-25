using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ticketsystem.Models;
using System.Threading.Tasks;
using System.Linq;
using Ticketsystem.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;

namespace Ticketsystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var tickets = await _context.Tickets.ToListAsync();
            var users = await _userManager.Users.ToListAsync();

            var model = new AdminTicketPanelViewModel
            {
                OpenTicketsCount = tickets.Count(t => t.Status == "Offen"),
                InProgressTicketsCount = tickets.Count(t => t.Status == "In Bearbeitung"),
                ClosedTicketsCount = tickets.Count(t => t.Status == "Geschlossen"),
                OverdueTicketsCount = tickets.Count(t => t.Status != "Geschlossen" && t.CreatedAt < DateTime.Now.AddDays(-3)),
                UserNames = users.Select(u => u.UserName).OfType<string>().ToList()
            };

            return View(model);
        }

        public async Task<IActionResult> Tickets(bool overdue = false)
        {
            var tickets = await _context.Tickets.Include(t => t.Creator).ToListAsync();
            if (overdue)
            {
                tickets = tickets.Where(t => t.Status != "Geschlossen" && t.CreatedAt < DateTime.Now.AddDays(-3)).ToList();
            }
            var grouped = tickets
                .GroupBy(t => t.Priority ?? "Unbekannt")
                .ToDictionary(g => g.Key, g => g.ToList());

            var model = new AdminTicketsByPriorityViewModel
            {
                GroupedTickets = grouped
            };
            return View(model);
        }
    }
} 