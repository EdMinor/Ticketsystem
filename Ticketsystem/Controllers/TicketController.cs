using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ticketsystem.Models;
using Ticketsystem.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.EntityFrameworkCore;

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

        // Filter für Tickets, die einem DevOps (Developer) zugewiesen sind
        public async Task<IActionResult> Index(string? status = null, bool overdue = false, bool assignedToDevOps = false)
        {
            IQueryable<Ticket> ticketsQuery = _context.Tickets
                .Include(t => t.Creator)
                .Include(t => t.Category)
                .Include(t => t.Zugewiesener);

            if (!string.IsNullOrEmpty(status))
                ticketsQuery = ticketsQuery.Where(t => t.Status == status);

            if (overdue)
                ticketsQuery = ticketsQuery.Where(t => t.Status != "Geschlossen" && t.CreatedAt < DateTime.Now.AddDays(-3));

            // Wenn assignedToDevOps aktiviert ist, nur Tickets anzeigen, die einem Benutzer mit der Rolle "Developer" zugewiesen sind
            if (assignedToDevOps)
            {
                // Hole alle UserIds mit Rolle "Developer"
                var devOpsUsers = await _userManager.GetUsersInRoleAsync("Developer");
                var devOpsUserIds = devOpsUsers.Select(u => u.Id).ToList();
                ticketsQuery = ticketsQuery.Where(t => t.ZugewiesenerId != null && devOpsUserIds.Contains(t.ZugewiesenerId));
            }

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


        // Ticket erstellen (GET)
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Titel");
            return View(new CreateTicketViewModel());
        }

        // Methode um die Unterordner für die Dateien zu erstellen
        private string GetUploadSubfolder(string ext)
        {
            switch (ext)
            {
                case ".jpg": case ".jpeg": case ".png": case ".gif":
                    return "images";
                case ".pdf":
                    return "pdf";
                case ".zip":
                    return "zip";
                default:
                    return "other";
            }
        }

        // Ticket erstellen (POST)
        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 5242880)] // 5MB pro Datei
        [RequestSizeLimit(5242880)]
        public async Task<IActionResult> Create(CreateTicketViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Titel", model.CategoryId);
                TempData["FileError"] = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return View(model);
            }

            var ticket = new Ticket
            {
                Title = model.Title,
                Description = model.Description,
                Priority = model.Priority,
                CategoryId = model.CategoryId,
                CreatorId = _userManager.GetUserId(User),
                CreatedAt = DateTime.Now,
                Status = "Offen"
            };
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            // Dateien verarbeiten
            if (model.NewFiles != null && model.NewFiles.Count > 0)
            {
                var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".gif", ".zip" };
                var wwwroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var baseUploadPath = Path.Combine(wwwroot, "uploads");
                if (!Directory.Exists(baseUploadPath)) Directory.CreateDirectory(baseUploadPath);

                foreach (var file in model.NewFiles)
                {
                    if (file == null || file.Length == 0) continue;
                    if (file.Length > 5 * 1024 * 1024)
                    {
                        TempData["FileError"] = $"Datei {file.FileName} ist größer als 5MB.";
                        continue;
                    }
                    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(ext))
                    {
                        TempData["FileError"] = $"Dateityp {ext} ist nicht erlaubt.";
                        continue;
                    }
                    try
                    {
                        var subfolder = GetUploadSubfolder(ext);
                        var uploadPath = Path.Combine(baseUploadPath, subfolder);
                        if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);
                        var storedName = Guid.NewGuid().ToString("N") + ext;
                        var filePath = Path.Combine(baseUploadPath, subfolder, storedName);
                        TempData["FileError"] = $"Versuche zu speichern: {filePath}";
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        var attachment = new TicketAttachment
                        {
                            TicketId = ticket.Id,
                            FileName = file.FileName,
                            StoredFileName = storedName,
                            ContentType = file.ContentType,
                            Size = file.Length,
                            UploadDate = DateTime.Now
                        };
                        _context.TicketAttachments.Add(attachment);
                    }
                    catch (Exception ex)
                    {
                        TempData["FileError"] = $"Fehler beim Hochladen von {file.FileName}: {ex.Message}";
                    }
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // Ticket bearbeiten (GET)
        public async Task<IActionResult> Edit(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var canDelete = ticket.CreatorId == userId || User.IsInRole("Admin");

            var attachments = await _context.TicketAttachments.Where(a => a.TicketId == id).ToListAsync();

            var model = new EditTicketViewModel
            {
                Id = ticket.Id,
                Title = ticket.Title ?? string.Empty,
                Description = ticket.Description ?? string.Empty,
                Priority = ticket.Priority ?? string.Empty,
                CategoryId = ticket.CategoryId,
                Attachments = attachments,
                CanDeleteAttachments = canDelete
            };

            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Titel", ticket.CategoryId);
            return View(model);
        }

        // Ticket bearbeiten (POST)
        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 5242880)] // 5MB pro Datei
        [RequestSizeLimit(5242880)]
        public async Task<IActionResult> Edit(EditTicketViewModel model)
        {
            var ticket = await _context.Tickets.FindAsync(model.Id);
            if (ticket == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");
            var isCreator = ticket.CreatorId == userId;
            var isAssignedDevOps = ticket.ZugewiesenerId == userId;
            // Nur Admin, Ersteller oder zugewiesener DevOps können Dateien hochladen
            if (!isAdmin && !isCreator && !isAssignedDevOps)
                return Forbid();

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Titel", model.CategoryId);
                model.Attachments = await _context.TicketAttachments.Where(a => a.TicketId == model.Id).ToListAsync();
                model.CanDeleteAttachments = ticket.CreatorId == userId || User.IsInRole("Admin");
                TempData["FileError"] = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return View(model);
            }

            ticket.Title = model.Title;
            ticket.Description = model.Description;
            ticket.Priority = model.Priority;
            ticket.CategoryId = model.CategoryId;

            // Dateien verarbeiten
            if (model.NewFiles != null && model.NewFiles.Count > 0)
            {
                var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".gif", ".zip" };
                var wwwroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var baseUploadPath = Path.Combine(wwwroot, "uploads");
                if (!Directory.Exists(baseUploadPath)) Directory.CreateDirectory(baseUploadPath);

                foreach (var file in model.NewFiles)
                {
                    if (file == null || file.Length == 0) continue;
                    if (file.Length > 5 * 1024 * 1024)
                    {
                        TempData["FileError"] = $"Datei {file.FileName} ist größer als 5MB.";
                        continue;
                    }
                    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(ext))
                    {
                        TempData["FileError"] = $"Dateityp {ext} ist nicht erlaubt.";
                        continue;
                    }
                    try
                    {
                        var subfolder = GetUploadSubfolder(ext);
                        var uploadPath = Path.Combine(baseUploadPath, subfolder);
                        if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);
                        var storedName = Guid.NewGuid().ToString("N") + ext;
                        var filePath = Path.Combine(baseUploadPath, subfolder, storedName);
                        TempData["FileError"] = $"Versuche zu speichern: {filePath}";
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        var attachment = new TicketAttachment
                        {
                            TicketId = ticket.Id,
                            FileName = file.FileName,
                            StoredFileName = storedName,
                            ContentType = file.ContentType,
                            Size = file.Length,
                            UploadDate = DateTime.Now
                        };
                        _context.TicketAttachments.Add(attachment);
                    }
                    catch (Exception ex)
                    {
                        TempData["FileError"] = $"Fehler beim Hochladen von {file.FileName}: {ex.Message}";
                    }
                }
            }

            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Edit), new { id = ticket.Id });
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

            // Kommentare zum Ticket laden (inkl. Benutzer)
            var comments = await _context.TicketComments
                .Where(c => c.TicketId == id)
                .Include(c => c.User)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();

            // Anhänge zum Ticket laden
            var attachments = await _context.TicketAttachments
                .Where(a => a.TicketId == id)
                .OrderBy(a => a.UploadDate)
                .ToListAsync();

            // Entwicklerliste für Admin
            if (User.IsInRole("Admin"))
            {
                ViewBag.Developers = await _userManager.GetUsersInRoleAsync("Developer");
            }

            var viewModel = new TicketDetailsViewModel
            {
                Ticket = ticket,
                Comments = comments,
                Attachments = attachments
            };

            return View(viewModel);
        }

        // Kommentar hinzufügen
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddComment(int ticketId, string newCommentText)
        {
            if (string.IsNullOrWhiteSpace(newCommentText) || newCommentText.Length > 2000)
            {
                TempData["CommentError"] = "Kommentar darf nicht leer sein und maximal 2000 Zeichen haben.";
                return RedirectToAction(nameof(Details), new { id = ticketId });
            }

            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var comment = new TicketComment
            {
                TicketId = ticketId,
                UserId = userId,
                Text = newCommentText,
                CreatedAt = DateTime.Now
            };
            _context.TicketComments.Add(comment);
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
            ticket.Status = "In Bearbeitung"; // Status auf 'In Bearbeitung' setzen

            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = ticketId });
        }

        // Fall schließen durch Developer
        [HttpPost]
        [Authorize(Roles = "Developer")]
        public async Task<IActionResult> CloseCase(int ticketId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            // Nur der zugewiesene Developer darf schließen
            if (ticket.ZugewiesenerId != currentUserId)
                return Forbid();

            ticket.Status = "Geschlossen"; // Status auf 'Geschlossen' setzen
            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = ticketId });
        }

        // Fall wieder eröffnen durch Admin oder Ersteller
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ReopenCase(int ticketId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            // Nur Admin oder Ersteller darf wieder eröffnen
            if (!User.IsInRole("Admin") && ticket.CreatorId != currentUserId)
                return Forbid();

            ticket.Status = "Offen"; // Status auf 'Offen' setzen
            ticket.CreatedAt = DateTime.Now; // Datum aktualisieren
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

        // Anhang löschen
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteAttachment(int id, int ticketId)
        {
            var attachment = await _context.TicketAttachments.FindAsync(id);
            if (attachment == null) return NotFound();

            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (ticket.CreatorId != userId && !User.IsInRole("Admin"))
                return Forbid();

            var ext = Path.GetExtension(attachment.FileName).ToLowerInvariant();
            var subfolder = GetUploadSubfolder(ext);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", subfolder, attachment.StoredFileName);
            try
            {
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }
            catch (Exception ex)
            {
                TempData["FileError"] = $"Fehler beim Löschen der Datei: {ex.Message}";
            }

            _context.TicketAttachments.Remove(attachment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Edit), new { id = ticketId });
        }

        // Datei herunterladen (nur für Admin, Ersteller, zugewiesener DevOps)
        [HttpGet]
        public async Task<IActionResult> DownloadAttachment(int id)
        {
            var attachment = await _context.TicketAttachments.Include(a => a.Ticket).FirstOrDefaultAsync(a => a.Id == id);
            if (attachment == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");
            var isCreator = attachment.Ticket.CreatorId == userId;
            var isAssignedDevOps = attachment.Ticket.ZugewiesenerId == userId;
            if (!isAdmin && !isCreator && !isAssignedDevOps)
                return Forbid();

            var ext = Path.GetExtension(attachment.FileName).ToLowerInvariant();
            var subfolder = GetUploadSubfolder(ext);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", subfolder, attachment.StoredFileName);
            if (!System.IO.File.Exists(filePath)) return NotFound();

            var contentType = attachment.ContentType ?? "application/octet-stream";
            return PhysicalFile(filePath, contentType, attachment.FileName);
        }

        // Datei-Vorschau (Modal, placeholder)
        [HttpGet]
        public IActionResult PreviewAttachment(int id) { return NotFound(); }
    }
}
