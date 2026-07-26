using RegulatoryComplianceApplication.Core.Interfaces;
using RegulatoryComplianceApplication.Core.Models;

namespace RegulatoryComplianceApplication.Infrastructure.Services
{
    public class CalendarService : ICalendarService
    {
        private readonly IDocumentService _documentService;
        private readonly IBillService _billService;

        public CalendarService(
            IDocumentService documentService,
            IBillService billService)
        {
            _documentService = documentService;
            _billService = billService;
        }

        public async Task<List<CalendarEvent>> GetEventsAsync()
        {
            var events = new List<CalendarEvent>();

            var documents = await _documentService.GetAllAsync();

            foreach (var document in documents)
            {
                if (document.CurrentVersion?.ExpiryDate == null)
                    continue;

                events.Add(new CalendarEvent
                {
                    Title = $" {document.Title}",
                    Start = document.CurrentVersion.ExpiryDate.Value.ToDateTime(TimeOnly.MinValue),
                    Color = "#f39c12",
                    Url = $"/Documents/Details/{document.DocumentId}"
                });
            }

            var bills = await _billService.GetAllAsync();

            foreach (var bill in bills)
            {
                var color = bill.Status switch
                {
                    Core.Entities.BillStatus.Overdue => "#dc3545",
                    Core.Entities.BillStatus.Pending => "#198754",
                    Core.Entities.BillStatus.Paid => "#0d6efd",
                    _ => "#6c757d"
                };

                events.Add(new CalendarEvent
                {
                    Title = $" {bill.BillName}",
                    Start = bill.DueDate.ToDateTime(TimeOnly.MinValue),
                    Color = color,
                    Url = $"/Bills/Details/{bill.BillId}"
                });
            }

            return events;
        }
    }
}