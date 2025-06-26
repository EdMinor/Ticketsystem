using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ticketsystem.ViewModels
{
    public class CreateUserViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
        public IEnumerable<string> Roles { get; set; } = new List<string>();

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;
    }
} 