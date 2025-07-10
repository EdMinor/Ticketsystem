using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Ticketsystem.Models;

namespace Ticketsystem.ViewModels
{
    // ViewModel für das Bearbeiten eines Tickets inkl. Anhänge
    public class EditTicketViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Priority { get; set; } = string.Empty;

        [Required]
        public int? CategoryId { get; set; }

        public List<TicketAttachment> Attachments { get; set; } = new();

        // Für neue Uploads
        public List<IFormFile> NewFiles { get; set; } = new();

        // Darf der aktuelle Benutzer Anhänge löschen?
        public bool CanDeleteAttachments { get; set; } = false;
    }
} 