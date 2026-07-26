namespace RegulatoryComplianceApplication.Core.Models
{
    public class CalendarEvent
    {
        public string Title { get; set; } = string.Empty;

        public DateTime Start { get; set; }

        public string Color { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;
    }
}