using System.ComponentModel.DataAnnotations;

namespace RegulatoryComplianceApplication.Web.ViewModels
{
    public class EditUserViewModel
    {
        public int UserId { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public int RoleId { get; set; }

        public bool IsActive { get; set; }
    }
}