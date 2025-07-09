using System.Collections.Generic;
using Ticketsystem.Models;

namespace Ticketsystem.ViewModels
{
    public class AdminTicketsByPriorityViewModel
    {
        // F�r die alte Gruppierung nach Priorit�t (falls weiterhin ben�tigt)
        public Dictionary<string, List<Ticket>> GroupedTickets { get; set; } = new();

        // Neu: Flache Liste aller Tickets (f�r DataTable)
        public List<Ticket> AllTickets { get; set; } = new();

        // Optional: F�r Filter-Dropdowns (falls du Kategorien oder Status vorfiltern willst)
        public List<string> Priorities { get; set; } = new() { "Hoch", "Mittel", "Niedrig" };
        public List<string> Statuses { get; set; } = new() { "Offen", "In Bearbeitung", "Geschlossen" };
        public List<Category> Categories { get; set; } = new();
    }
}
