using Microsoft.AspNetCore.Mvc.Rendering;

namespace RegulatoryComplianceApplication.Web.ViewModels
{
    public class CreateDocumentViewModel
    {
        public int DocumentTypeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty;
        public DateOnly IssueDate { get; set; }
        public DateOnly? ExpiryDate { get; set; }
        public IFormFile? File { get; set; }

        public List<SelectListItem> DocumentTypes { get; set; } = new();
        public bool IsExpirable { get; set; } = true;
    }
}