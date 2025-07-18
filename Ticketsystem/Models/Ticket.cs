﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

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

        public int? CategoryId { get; set; }
        public virtual Category? Category { get; set; }

        public string? ZugewiesenerId { get; set; } = null;
        public virtual ApplicationUser? Zugewiesener { get; set; }

        public DateTime? ZugewiesenAm { get; set; } = null;

        public string? CreatorId { get; set; } = null;

        public virtual ApplicationUser? Creator { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
