using RegulatoryComplianceApplication.Core.Entities;
using RegulatoryComplianceApplication.Core.Interfaces;
using RegulatoryComplianceApplication.Infrastructure.Data;

namespace RegulatoryComplianceApplication.Infrastructure.Services
{
    public class AuditLogger : IAuditLogger
    {
        private readonly AppDbContext _context;

        public AuditLogger(AppDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(int actingUserId, string action, string entityName, int entityId,
            string? fieldName = null, string? oldValue = null, string? newValue = null)
        {
            _context.AuditLogs.Add(new AuditLog
            {
                UserId = actingUserId,
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                FieldName = fieldName,
                OldValue = oldValue,
                NewValue = newValue,
                Timestamp = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
        }
    }
}