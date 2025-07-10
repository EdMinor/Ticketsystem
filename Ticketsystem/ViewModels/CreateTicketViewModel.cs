using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Ticketsystem.ViewModels
{
    // ViewModel für das Erstellen eines Tickets inkl. Datei-Upload
    public class CreateTicketViewModel
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Priority { get; set; } = string.Empty;

        [Required]
        public int? CategoryId { get; set; }

        // Für neue Uploads
        public List<IFormFile> NewFiles { get; set; } = new();
    }
} 