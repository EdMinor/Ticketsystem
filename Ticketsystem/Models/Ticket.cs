using System.ComponentModel.DataAnnotations;

namespace Ticketsystem.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        [Required]
        public string? Title { get; set; } = null;

        [Required]
        public string? Description { get; set; } = null;

        [Required]
        public string? Status { get; set; } = "Offen"; // Default

        [Required]
        public string? Priority { get; set; } = null;

        public string? Category { get; set; } = null;

        public string? CreatorId { get; set; } = null;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
