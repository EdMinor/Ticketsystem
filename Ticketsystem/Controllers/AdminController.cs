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

        public async Task<IActionResult> Tickets(bool overdue = false, string status = null)
        {
            var tickets = await _context.Tickets.Include(t => t.Creator).ToListAsync();
            if (!string.IsNullOrEmpty(status))
            {
                tickets = tickets.Where(t => t.Status == status).ToList();
            }
            if (overdue)
            {
                tickets = tickets.Where(t => t.Status != "Geschlossen" && t.CreatedAt < DateTime.Now.AddDays(-3)).ToList();
            }
            var priorities = new[] { "Hoch", "Mittel", "Niedrig" };
            var grouped = tickets
                .GroupBy(t => t.Priority ?? "Unbekannt")
                .ToDictionary(g => g.Key, g =>
                {
                    string sortField = Request.Query[$"sortField_{g.Key}"].ToString();
                    string sortOrder = Request.Query[$"sortOrder_{g.Key}"].ToString();
                    if (string.IsNullOrEmpty(sortField)) sortField = "date";
                    if (string.IsNullOrEmpty(sortOrder)) sortOrder = "desc";
                    IEnumerable<Ticket> sorted = g;
                    switch (sortField)
                    {
                        case "name":
                            sorted = sortOrder == "asc" ? g.OrderBy(t => t.Creator?.UserName) : g.OrderByDescending(t => t.Creator?.UserName);
                            break;
                        case "status":
                            sorted = sortOrder == "asc" ? g.OrderBy(t => t.Status) : g.OrderByDescending(t => t.Status);
                            break;
                        case "date":
                        default:
                            sorted = sortOrder == "asc" ? g.OrderBy(t => t.CreatedAt) : g.OrderByDescending(t => t.CreatedAt);
                            break;
                    }
                    return sorted.ToList();
                });

            foreach (var p in priorities)
            {
                var sortFieldVal = Request.Query[$"sortField_{p}"].ToString();
                var sortOrderVal = Request.Query[$"sortOrder_{p}"].ToString();
                switch (p)
                {
                    case "Hoch":
                        ViewBag.SortField_Hoch = string.IsNullOrEmpty(sortFieldVal) ? "date" : sortFieldVal;
                        ViewBag.SortOrder_Hoch = string.IsNullOrEmpty(sortOrderVal) ? "desc" : sortOrderVal;
                        break;
                    case "Mittel":
                        ViewBag.SortField_Mittel = string.IsNullOrEmpty(sortFieldVal) ? "date" : sortFieldVal;
                        ViewBag.SortOrder_Mittel = string.IsNullOrEmpty(sortOrderVal) ? "desc" : sortOrderVal;
                        break;
                    case "Niedrig":
                        ViewBag.SortField_Niedrig = string.IsNullOrEmpty(sortFieldVal) ? "date" : sortFieldVal;
                        ViewBag.SortOrder_Niedrig = string.IsNullOrEmpty(sortOrderVal) ? "desc" : sortOrderVal;
                        break;
                }
            }

            var model = new AdminTicketsByPriorityViewModel
            {
                GroupedTickets = grouped
            };
            return View(model);
        }
    }
} 