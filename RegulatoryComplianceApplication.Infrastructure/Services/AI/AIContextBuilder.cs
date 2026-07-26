using Microsoft.EntityFrameworkCore;
using RegulatoryComplianceApplication.Core.Entities;
using RegulatoryComplianceApplication.Core.Interfaces;
using RegulatoryComplianceApplication.Core.Models;
using RegulatoryComplianceApplication.Infrastructure.Data;
using System.Text;

namespace RegulatoryComplianceApplication.Infrastructure.Services.AI
{
    public class AIContextBuilder : IAIContextBuilder
    {
        private readonly AppDbContext _context;
        private readonly IDocumentService _documentService;
        private readonly IBillService _billService;

        public AIContextBuilder(
            AppDbContext context,
            IDocumentService documentService,
            IBillService billService)
        {
            _context = context;
            _documentService = documentService;
            _billService = billService;
        }

        public async Task<string> BuildContextAsync(
            AssistantIntent intent,
            string question)
        {
            var sb = new StringBuilder();

            switch (intent)
            {
                case AssistantIntent.GeneralKnowledge:

                    return "";

                case AssistantIntent.Documents:

                    await AddDocuments(sb);
                    break;

                case AssistantIntent.ExpiringDocuments:

                    await AddExpiringDocuments(sb);
                    break;

                case AssistantIntent.DocumentsByUser:

                    await AddDocumentsByUser(sb, question);
                    break;

                case AssistantIntent.Bills:

                    await AddBills(sb);
                    break;

                case AssistantIntent.OverdueBills:

                    await AddOverdueBills(sb);
                    break;

                case AssistantIntent.DueSoonBills:

                    await AddDueSoonBills(sb);
                    break;

                case AssistantIntent.Users:

                    await AddUsers(sb);
                    break;

                case AssistantIntent.Notifications:

                    await AddNotifications(sb);
                    break;

                case AssistantIntent.AuditLogs:

                    await AddAuditLogs(sb);
                    break;

                case AssistantIntent.Dashboard:

                    await AddDashboard(sb);
                    break;
            }

            return sb.ToString();
        }

        private async Task AddDocuments(StringBuilder sb)
        {
            var docs = await _documentService.GetAllAsync();

            sb.AppendLine("Documents:");

            foreach (var d in docs)
            {
                sb.AppendLine(
                    $"{d.Title} | {d.DocumentNumber}");
            }
        }

        private async Task AddExpiringDocuments(StringBuilder sb)
        {
            var docs = await _documentService.GetExpiringSoonAsync(60);

            sb.AppendLine("Documents expiring within 60 days:");

            foreach (var d in docs)
            {
                sb.AppendLine($"""
                    Title: {d.Title}
                    Number: {d.DocumentNumber}
                    Expiry: {d.CurrentVersion?.ExpiryDate}

                    """);
            }
        }

        private async Task AddDocumentsByUser(
            StringBuilder sb,
            string question)
        {
            var users = await _context.Users
                .Where(u => u.IsActive)
                .ToListAsync();

            var matchedUser = users.FirstOrDefault(u =>
            {
                var fullName = u.FullName.ToLowerInvariant();
                var firstName = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

                var q = question.ToLowerInvariant();

                return q.Contains(fullName) ||
                       (firstName != null && q.Contains(firstName));
            });

            if (matchedUser == null)
            {
                sb.AppendLine("No matching user found.");
                return;
            }

            var docs = await _context.DocumentResponsibleUsers
                .Include(r => r.Document)
                    .ThenInclude(d => d.CurrentVersion)
                .Include(r => r.User)
                .Where(r => r.UserId == matchedUser.UserId)
                .ToListAsync();

            sb.AppendLine(
                $"Documents assigned to {matchedUser.FullName}:");

            foreach (var d in docs)
            {
                sb.AppendLine($"""
Title: {d.Document.Title}
Number: {d.Document.DocumentNumber}
Expiry: {d.Document.CurrentVersion?.ExpiryDate}

""");
            }
        }

        private async Task AddBills(StringBuilder sb)
        {
            var bills = await _billService.GetAllAsync();

            foreach (var bill in bills)
            {
                sb.AppendLine(
                    $"{bill.BillName} | {bill.Status}");
            }
        }

        private async Task AddOverdueBills(StringBuilder sb)
        {
            var bills = await _billService.GetOverdueAsync();

            foreach (var bill in bills)
            {
                sb.AppendLine(
                    $"{bill.BillName} | {bill.Amount} SAR");
            }
        }

        private async Task AddDueSoonBills(StringBuilder sb)
        {
            var bills = await _billService.GetDueSoonAsync(30);

            foreach (var bill in bills)
            {
                sb.AppendLine(
                    $"{bill.BillName} | Due {bill.DueDate}");
            }
        }

        private async Task AddUsers(StringBuilder sb)
        {
            var users = await _context.Users
                .Where(u => u.IsActive)
                .ToListAsync();

            foreach (var u in users)
            {
                sb.AppendLine(
                    $"{u.FullName} ({u.Email})");
            }
        }

        private async Task AddNotifications(StringBuilder sb)
        {
            var notifications = await _context.Notifications
                .OrderByDescending(n => n.CreatedAt)
                .Take(10)
                .ToListAsync();

            foreach (var n in notifications)
            {
                sb.AppendLine(n.Message);
            }
        }

        private async Task AddAuditLogs(StringBuilder sb)
        {
            var logs = await _context.AuditLogs
                .Include(a => a.User)
                .OrderByDescending(a => a.Timestamp)
                .Take(10)
                .ToListAsync();

            foreach (var log in logs)
            {
                sb.AppendLine(
                    $"{log.User.FullName} {log.Action} {log.EntityName}");
            }
        }

        private async Task AddDashboard(StringBuilder sb)
        {
            sb.AppendLine(
                $"Documents: {await _context.Documents.CountAsync(d => !d.IsDeleted)}");

            sb.AppendLine(
                $"Bills: {await _context.Bills.CountAsync(b => !b.IsDeleted)}");

            sb.AppendLine(
                $"Users: {await _context.Users.CountAsync(u => u.IsActive)}");

            sb.AppendLine(
                $"Notifications: {await _context.Notifications.CountAsync()}");
        }
    }
}