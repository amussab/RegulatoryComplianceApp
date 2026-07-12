using System.ComponentModel.DataAnnotations;

namespace RegulatoryComplianceApplication.Web.ViewModels
{
    public class RenewDocumentViewModel
    {
        public int DocumentId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string DocumentNumber { get; set; } = string.Empty;

        [Required]
        public DateOnly? NewExpiryDate { get; set; }

        [Required]
        public IFormFile? File { get; set; }
    }
}