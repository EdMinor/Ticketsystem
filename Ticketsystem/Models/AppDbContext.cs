﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;

namespace Ticketsystem.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<TicketComment> TicketComments { get; set; } // Kommentare zu Tickets
        public DbSet<TicketAttachment> TicketAttachments { get; set; } // Anhänge/Dateien zu Tickets
    }
}
