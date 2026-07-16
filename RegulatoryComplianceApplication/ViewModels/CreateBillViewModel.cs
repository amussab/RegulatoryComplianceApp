using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using RegulatoryComplianceApplication.Core.Entities;

namespace RegulatoryComplianceApplication.Web.ViewModels
{
    public class CreateBillViewModel
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public string BillName { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public DateOnly DueDate { get; set; }

        public BillFrequency Frequency { get; set; }

        public bool IsRecurring { get; set; }

        public BillStatus Status { get; set; } = BillStatus.Pending;

        public IFormFile? Attachment { get; set; }

        public List<SelectListItem> Users { get; set; } = new();
    }
}