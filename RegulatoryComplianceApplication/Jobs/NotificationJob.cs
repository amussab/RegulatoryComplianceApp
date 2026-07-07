using RegulatoryComplianceApplication.Core.Interfaces;

namespace RegulatoryComplianceApplication.Jobs
{
    public class NotificationJob
    {
        private readonly IDocumentService _documentService;
        private readonly INotificationService _notificationService;

        public NotificationJob(
            IDocumentService documentService,
            INotificationService notificationService)
        {
            _documentService = documentService;
            _notificationService = notificationService;
        }

        public async Task ExecuteAsync()
        {
            var expiringDocuments = await _documentService.GetExpiringSoonAsync(60);

            foreach (var document in expiringDocuments)
            {
                await _notificationService.CreateExpiryNotificationsAsync(document);
            }
        }
    }
}