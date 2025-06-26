using Microsoft.AspNetCore.Identity;
namespace Ticketsystem.Models{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
    }
}