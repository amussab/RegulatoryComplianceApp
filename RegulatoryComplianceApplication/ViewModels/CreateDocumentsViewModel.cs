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

        public int ResponsibleUserId { get; set; }

        public List<SelectListItem> DocumentTypes { get; set; } = new();

        public List<SelectListItem> Users { get; set; } = new();
        public List<int> ExpirableDocumentTypeIds { get; set; } = new();
    }
}