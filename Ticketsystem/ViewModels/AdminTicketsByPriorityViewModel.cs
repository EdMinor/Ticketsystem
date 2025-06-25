using System.Collections.Generic;
using Ticketsystem.Models;

namespace Ticketsystem.ViewModels
{
    public class AdminTicketsByPriorityViewModel
    {
        public Dictionary<string, List<Ticket>> GroupedTickets { get; set; } = new();
    }
} 