using System;
using System.ComponentModel.DataAnnotations;

namespace Ticketsystem.Models
{
    // Anhang/Datei zu einem Ticket
    public class TicketAttachment
    {
        [Key]
        public int Id { get; set; }

        // Zugehöriges Ticket
        [Required]
        public int TicketId { get; set; }
        public virtual Ticket Ticket { get; set; } = null!;

        // Ursprünglicher Dateiname
        [Required]
        public string FileName { get; set; } = string.Empty;

        // Name der Datei auf dem Server (eindeutig)
        [Required]
        public string StoredFileName { get; set; } = string.Empty;

        // MIME-Typ
        [Required]
        public string ContentType { get; set; } = string.Empty;

        // Dateigröße in Bytes
        public long Size { get; set; }

        // Upload-Datum
        public DateTime UploadDate { get; set; } = DateTime.Now;
    }
} 