using System.ComponentModel.DataAnnotations;

namespace Ticketsystem.ViewModels
{
    public class EditCategoryViewModel
    {
        public string OldName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string NewName { get; set; } = string.Empty;
    }
} 