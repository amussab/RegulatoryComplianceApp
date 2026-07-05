namespace RegulatoryComplianceApplication.Core.Interfaces
{
    public interface IAuditLogger
    {
        Task LogAsync(int actingUserId, string action, string entityName, int entityId,
            string? fieldName = null, string? oldValue = null, string? newValue = null);
    }
}