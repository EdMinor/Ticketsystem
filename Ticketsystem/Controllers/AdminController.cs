using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ticketsystem.Models;
using System.Threading.Tasks;
using System.Linq;
using Ticketsystem.ViewModels;

namespace Ticketsystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string sortOrder = "date", string sortDirection = "desc")
        {
            var tickets = await _context.Tickets.Include(t => t.Creator).ToListAsync();

            // Сортировка
            tickets = sortOrder switch
            {
                "status" => sortDirection == "asc" ? tickets.OrderBy(t => t.Status).ToList() : tickets.OrderByDescending(t => t.Status).ToList(),
                "user" => sortDirection == "asc" ? tickets.OrderBy(t => t.Creator?.UserName).ToList() : tickets.OrderByDescending(t => t.Creator?.UserName).ToList(),
                _ => sortDirection == "asc" ? tickets.OrderBy(t => t.CreatedAt).ToList() : tickets.OrderByDescending(t => t.CreatedAt).ToList(),
            };

            // Группировка по приоритету
            var grouped = tickets.GroupBy(t => t.Priority ?? "Unbekannt")
                .ToDictionary(g => g.Key, g => g.ToList());

            var model = new AdminTicketPanelViewModel
            {
                GroupedTickets = grouped,
                SortOrder = sortOrder,
                SortDirection = sortDirection
            };

            return View(model);
        }
    }
} 