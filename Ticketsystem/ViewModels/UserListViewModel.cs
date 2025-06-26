using System.Collections.Generic;

namespace Ticketsystem.ViewModels
{
    public class UserListViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public IEnumerable<string> Roles { get; set; } = new List<string>();
        public string FullName { get; set; } = string.Empty;
    }
} 