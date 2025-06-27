using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ticketsystem.Models;

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

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var tickets = await _context.Tickets
                .Where(t => t.CreatorId == userId || User.IsInRole("Admin"))
                .ToListAsync();

            return View(tickets);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Name", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Ticket ticket)
        {
            if (!ModelState.IsValid) return View(ticket);

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

            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Name", "Name", ticket.Category);
            return View(ticket);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Ticket ticket)
        {
            if (!ModelState.IsValid) return View(ticket);

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
            var ticket = await _context.Tickets.FindAsync(id);
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

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
