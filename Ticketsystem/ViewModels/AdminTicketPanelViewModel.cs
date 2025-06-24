using System.Collections.Generic;
using Ticketsystem.Models;

namespace Ticketsystem.ViewModels
{
    public class AdminTicketPanelViewModel
    {
        public Dictionary<string, List<Ticket>> GroupedTickets { get; set; } = new();
        public string SortOrder { get; set; } = "date";
        public string SortDirection { get; set; } = "desc";
    }
} 