using RegulatoryComplianceApplication.Core.Entities;

namespace RegulatoryComplianceApplication.Core.Interfaces
{
    public interface INotificationService
    {
        Task CreateExpiryNotificationsAsync(Document document);
        Task CreateBillNotificationsAsync(Bill bill);

        Task<List<Notification>> GetUnreadNotificationsAsync(int userId);

        Task<List<Notification>> GetAllNotificationsAsync(int userId);

        Task<int> GetUnreadCountAsync(int userId);

        Task MarkAsReadAsync(int notificationId, int userId);

        Task MarkAllAsReadAsync(int userId);
    }
}