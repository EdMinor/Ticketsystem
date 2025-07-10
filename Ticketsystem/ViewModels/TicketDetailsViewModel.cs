using System.Collections.Generic;
using Ticketsystem.Models;

namespace Ticketsystem.ViewModels
{
    // ViewModel für die Ticket-Detailseite inkl. Kommentare
    public class TicketDetailsViewModel
    {
        public Ticket Ticket { get; set; } = null!;
        public List<TicketComment> Comments { get; set; } = new();
        public string NewCommentText { get; set; } = string.Empty;
        public List<TicketAttachment> Attachments { get; set; } = new();
    }
} 