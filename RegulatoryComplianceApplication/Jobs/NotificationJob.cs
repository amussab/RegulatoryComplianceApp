using RegulatoryComplianceApplication.Core.Interfaces;

namespace RegulatoryComplianceApplication.Jobs
{
    public class NotificationJob
    {
        private readonly IDocumentService _documentService;
        private readonly INotificationService _notificationService;
        private readonly IBillService _billService;

        public NotificationJob(
             IDocumentService documentService,
             IBillService billService,
             INotificationService notificationService)
        {
            _documentService = documentService;
            _billService = billService;
            _notificationService = notificationService;
        }

        public async Task ExecuteAsync()
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();



            var expiringDocuments = await _documentService.GetExpiringSoonAsync(60);
           
            foreach (var document in expiringDocuments)
            {
                await _notificationService.CreateExpiryNotificationsAsync(document);
            }

           
            var dueSoonBills = await _billService.GetDueSoonAsync(7);
            
            foreach (var bill in dueSoonBills)
            {
                await _notificationService.CreateBillNotificationsAsync(bill);
            }

           
            var overdueBills = await _billService.GetOverdueAsync();
            
            foreach (var bill in overdueBills)
            {
                await _notificationService.CreateBillNotificationsAsync(bill);
            }
        }
    }
}