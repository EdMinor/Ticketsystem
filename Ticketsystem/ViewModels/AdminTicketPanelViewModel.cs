using System.Collections.Generic;
using Ticketsystem.Models;

namespace Ticketsystem.ViewModels
{
    public class AdminTicketPanelViewModel
    {
        public int OpenTicketsCount { get; set; }
        public int InProgressTicketsCount { get; set; }
        public int ClosedTicketsCount { get; set; }
        public List<string> UserNames { get; set; } = new();
        public int OverdueTicketsCount { get; set; }
    }
} 