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
            IQueryable<Ticket> ticketsQuery = _context.Tickets.Include(t => t.Creator).Include(t => t.Category);

            if (!string.IsNullOrEmpty(status))
                ticketsQuery = ticketsQuery.Where(t => t.Status == status);

            if (overdue)
                ticketsQuery = ticketsQuery.Where(t => t.Status != "Geschlossen" && t.CreatedAt < DateTime.Now.AddDays(-3));

            var tickets = await ticketsQuery.ToListAsync();
            var priorities = new[] { "Hoch", "Mittel", "Niedrig" };
            var grouped = tickets
                .GroupBy(t => t.Priority ?? "Unbekannt")
                .ToDictionary(g => g.Key, g => g.ToList());

            var viewModel = new AdminTicketsByPriorityViewModel
            {
                GroupedTickets = grouped
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
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
                return NotFound();

            return View(ticket);
        }
    }
}
