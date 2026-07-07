using Microsoft.EntityFrameworkCore;
using RegulatoryComplianceApplication.Core.Entities;
using RegulatoryComplianceApplication.Core.Interfaces;
using RegulatoryComplianceApplication.Infrastructure.Data;

namespace RegulatoryComplianceApplication.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private const string ManagerRoleName = "Management";
        private const string NotificationChannel = "InApp";
        private const string EmailSubject = "Document Expiry Reminder";

        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public NotificationService(
            AppDbContext context,
            IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task CreateExpiryNotificationsAsync(Document document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            var loadedDocument = await LoadDocumentAsync(document.DocumentId);

            if (loadedDocument == null ||
                loadedDocument.CurrentVersion == null)
            {
                return;
            }

            var recipients = await GetRecipientsAsync(loadedDocument);

            if (!recipients.Any())
            {
                return;
            }

            var message = BuildNotificationMessage(loadedDocument);

            var notificationsToCreate = new List<Notification>();
            var emailRecipients = new List<User>();

            var existingRecipientIds = await _context.Notifications
            .Where(n =>
            n.DocumentId == loadedDocument.DocumentId &&
            !n.IsRead)
            .Select(n => n.RecipientUserId)
            .ToHashSetAsync();

            foreach (var recipient in recipients)
            {
                if (existingRecipientIds.Contains(recipient.UserId))
                {
                    continue;
                }

                notificationsToCreate.Add(new Notification
                {
                    DocumentId = loadedDocument.DocumentId,
                    RecipientUserId = recipient.UserId,
                    Channel = NotificationChannel,
                    Message = message,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                });

                emailRecipients.Add(recipient);
            }
            if (!notificationsToCreate.Any())
            {
                return;
            }

            _context.Notifications.AddRange(notificationsToCreate);

            await _context.SaveChangesAsync();

            foreach (var notification in notificationsToCreate)
            {
                var recipient = emailRecipients.First(u =>
                    u.UserId == notification.RecipientUserId);

                try
                {
                    await _emailService.SendEmailAsync(
                        recipient.Email,
                        EmailSubject,
                        message);

                    notification.SentAt = DateTime.UtcNow;
                }
                catch
                {
                    // Email delivery is best-effort only.
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<Notification>> GetUnreadNotificationsAsync(int userId)
        {
            return await _context.Notifications
                .Include(n => n.Document)
                .ThenInclude(d => d.DocumentType)
                .Where(n =>
                    n.RecipientUserId == userId &&
                    !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Notification>> GetAllNotificationsAsync(int userId)
        {
            return await _context.Notifications
                .Include(n => n.Document)
                .ThenInclude(d => d.DocumentType)
                .Where(n => n.RecipientUserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _context.Notifications.CountAsync(n =>
                n.RecipientUserId == userId &&
                !n.IsRead);
        }
        public async Task MarkAsReadAsync(int notificationId, int userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n =>
                    n.NotificationId == notificationId &&
                    n.RecipientUserId == userId);

            if (notification == null)
            {
                return;
            }

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsReadAsync(int userId)
        {
            var notifications = await _context.Notifications
                .Where(n =>
                    n.RecipientUserId == userId &&
                    !n.IsRead)
                .ToListAsync();

            if (!notifications.Any())
            {
                return;
            }

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();
        }

        private async Task<Document?> LoadDocumentAsync(int documentId)
        {
            return await _context.Documents
                .Include(d => d.DocumentType)
                .Include(d => d.CurrentVersion)
                .Include(d => d.ResponsibleUsers)
                    .ThenInclude(ru => ru.User)
                .FirstOrDefaultAsync(d =>
                    d.DocumentId == documentId &&
                    !d.IsDeleted);
        }

        private async Task<List<User>> GetRecipientsAsync(Document document)
        {
            var responsibleUsers = document.ResponsibleUsers
                .Select(ru => ru.User);

            var managers = await _context.Users
                .Include(u => u.Role)
                .Where(u =>
                    u.IsActive &&
                    u.Role.RoleName == ManagerRoleName)
                .ToListAsync();

            return responsibleUsers
                .Concat(managers)
                .GroupBy(u => u.UserId)
                .Select(g => g.First())
                .ToList();
        }

        private async Task<bool> NotificationExistsAsync(
            int documentId,
            int recipientUserId)
        {
            return await _context.Notifications.AnyAsync(n =>
                n.DocumentId == documentId &&
                n.RecipientUserId == recipientUserId &&
                !n.IsRead);
        }

        private static string BuildNotificationMessage(Document document)
        {
            var expiryDate = document.CurrentVersion!.ExpiryDate
                .ToDateTime(TimeOnly.MinValue);

            return
                $"{document.DocumentType.TypeName}{Environment.NewLine}{Environment.NewLine}" +
                $"Document Number: {document.DocumentNumber}{Environment.NewLine}{Environment.NewLine}" +
                $"This document expires on {expiryDate:dd MMMM yyyy}.";
        }
    }
}