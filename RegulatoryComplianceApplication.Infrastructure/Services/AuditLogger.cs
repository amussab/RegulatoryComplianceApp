using RegulatoryComplianceApplication.Core.Entities;
using RegulatoryComplianceApplication.Core.Interfaces;
using RegulatoryComplianceApplication.Infrastructure.Data;
using System.Reflection;

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
        public async Task LogChangesAsync<T>(
    int actingUserId,
    string action,
    string entityName,
    int entityId,
    T oldEntity,
    T newEntity,
    params string[] includedProperties)
        {
            if (oldEntity == null || newEntity == null)
                return;

            var properties = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => includedProperties.Contains(p.Name));

            foreach (var property in properties)
            {
                var oldValue = property.GetValue(oldEntity)?.ToString();
                var newValue = property.GetValue(newEntity)?.ToString();

                if (oldValue == newValue)
                    continue;

                _context.AuditLogs.Add(new AuditLog
                {
                    UserId = actingUserId,
                    Action = action,
                    EntityName = entityName,
                    EntityId = entityId,
                    FieldName = property.Name,
                    OldValue = oldValue,
                    NewValue = newValue,
                    Timestamp = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
        }
    }
}