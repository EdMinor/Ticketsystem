using System;
using System.ComponentModel.DataAnnotations;

namespace Ticketsystem.Models
{
    // Kommentar zu einem Ticket
    public class TicketComment
    {
        [Key]
        public int Id { get; set; }

        // Zugehöriges Ticket
        [Required]
        public int TicketId { get; set; }
        public virtual Ticket Ticket { get; set; } = null!;

        // Benutzer, der den Kommentar erstellt hat
        [Required]
        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser User { get; set; } = null!;

        // Kommentartext
        [Required]
        [StringLength(2000, ErrorMessage = "Der Kommentar darf maximal 2000 Zeichen lang sein.")]
        public string Text { get; set; } = string.Empty;

        // Erstellungsdatum
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
} 