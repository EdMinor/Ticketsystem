using System.ComponentModel.DataAnnotations;

namespace Ticketsystem.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Titel { get; set; }

        public string Beschreibung { get; set; }

        [DataType(DataType.Date)]
        public DateTime Startdatum { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Enddatum { get; set; }
    }
} 