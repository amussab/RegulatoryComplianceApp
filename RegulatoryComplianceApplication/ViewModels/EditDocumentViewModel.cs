using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace RegulatoryComplianceApplication.Web.ViewModels
{
    public class EditDocumentViewModel
    {
        public int DocumentId { get; set; }

        [Required]
        public int DocumentTypeId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string DocumentNumber { get; set; } = string.Empty;

        [Required]
        public DateOnly IssueDate { get; set; }

        public DateOnly? ExpiryDate { get; set; }

        public int ResponsibleUserId { get; set; }

        public IFormFile? File { get; set; }

        public List<SelectListItem> DocumentTypes { get; set; } = new();

        public List<SelectListItem> Users { get; set; } = new();

        public string CurrentFilePath { get; set; } = string.Empty;
    }
}