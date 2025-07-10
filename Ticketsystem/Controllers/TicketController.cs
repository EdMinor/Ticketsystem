using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ticketsystem.Models;
using Ticketsystem.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace Ticketsystem.Controllers
{
    [Authorize]
    public class TicketController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TicketController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string? status = null, bool overdue = false)
        {
            IQueryable<Ticket> ticketsQuery = _context.Tickets
                .Include(t => t.Creator)
                .Include(t => t.Category)
                .Include(t => t.Zugewiesener);

            if (!string.IsNullOrEmpty(status))
                ticketsQuery = ticketsQuery.Where(t => t.Status == status);

            if (overdue)
                ticketsQuery = ticketsQuery.Where(t => t.Status != "Geschlossen" && t.CreatedAt < DateTime.Now.AddDays(-3));

            var tickets = await ticketsQuery.ToListAsync();

            var grouped = tickets
                .GroupBy(t => t.Priority ?? "Unbekannt")
                .ToDictionary(g => g.Key, g => g.ToList());

            var categories = await _context.Categories.ToListAsync();

            var viewModel = new AdminTicketsByPriorityViewModel
            {
                AllTickets = tickets,
                GroupedTickets = grouped,
                Categories = categories
            };

            return View(viewModel);
        }


        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Titel");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Ticket ticket)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Titel", ticket.CategoryId);
                return View(ticket);
            }

            ticket.CreatorId = _userManager.GetUserId(User);
            ticket.CreatedAt = DateTime.Now;
            ticket.Status = "Offen";

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (ticket.CreatorId != userId && !User.IsInRole("Admin"))
                return Forbid();

            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Titel", ticket.CategoryId);
            return View(ticket);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Ticket ticket)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Titel", ticket.CategoryId);
                return View(ticket);
            }

            var original = await _context.Tickets.AsNoTracking().FirstOrDefaultAsync(t => t.Id == ticket.Id);
            if (original == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (original.CreatorId != userId && !User.IsInRole("Admin"))
                return Forbid();

            ticket.CreatorId = original.CreatorId;
            ticket.CreatedAt = original.CreatedAt;

            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var ticket = await _context.Tickets.Include(t => t.Creator).Include(t => t.Category).FirstOrDefaultAsync(t => t.Id == id);
            if (ticket == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (ticket.CreatorId != userId && !User.IsInRole("Admin"))
                return Forbid();

            return View(ticket);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (ticket.CreatorId != userId && !User.IsInRole("Admin"))
                return Forbid();

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Creator)
                .Include(t => t.Category)
                .Include(t => t.Zugewiesener)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
                return NotFound();

            // Получаем список разработчиков для формы назначения
            if (User.IsInRole("Admin"))
            {
                ViewBag.Developers = await _userManager.GetUsersInRoleAsync("Developer");
            }

            return View(ticket);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignDeveloper(int ticketId, string developerId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null) return NotFound();

            var developer = await _userManager.FindByIdAsync(developerId);
            if (developer == null) return NotFound();

            ticket.ZugewiesenerId = developerId;
            ticket.ZugewiesenAm = DateTime.Now;

            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = ticketId });
        }

        [HttpPost]
        [Authorize(Roles = "Developer")]
        public async Task<IActionResult> TakeOverCase(int ticketId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            ticket.ZugewiesenerId = currentUserId;
            ticket.ZugewiesenAm = DateTime.Now;

            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = ticketId });
        }

        [HttpGet]
        public async Task<IActionResult> GetDevelopers()
        {
            var developers = await _userManager.GetUsersInRoleAsync("Developer");
            return Json(developers.Select(d => new { id = d.Id, name = d.UserName }));
        }
    }
}
