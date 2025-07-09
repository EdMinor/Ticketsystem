using System.Collections.Generic;
using Ticketsystem.Models;

namespace Ticketsystem.ViewModels
{
    public class AdminTicketsByPriorityViewModel
    {
        // Für die alte Gruppierung nach Priorität (falls weiterhin benötigt)
        public Dictionary<string, List<Ticket>> GroupedTickets { get; set; } = new();

        // Neu: Flache Liste aller Tickets (für DataTable)
        public List<Ticket> AllTickets { get; set; } = new();

        // Optional: Für Filter-Dropdowns (falls du Kategorien oder Status vorfiltern willst)
        public List<string> Priorities { get; set; } = new() { "Hoch", "Mittel", "Niedrig" };
        public List<string> Statuses { get; set; } = new() { "Offen", "In Bearbeitung", "Geschlossen" };
        public List<Category> Categories { get; set; } = new();
    }
}
