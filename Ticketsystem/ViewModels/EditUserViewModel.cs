using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ticketsystem.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        public IList<string> Roles { get; set; } = new List<string>();
    }
} 