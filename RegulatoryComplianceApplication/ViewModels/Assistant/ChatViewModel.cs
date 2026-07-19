using System.ComponentModel.DataAnnotations;

namespace RegulatoryComplianceApplication.ViewModels.Assistant
{
    public class ChatViewModel
    {
        [Required]
        [Display(Name = "Ask a question")]
        public string Question { get; set; } = string.Empty;

        public string? Response { get; set; }
    }
}