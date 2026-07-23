using RegulatoryComplianceApplication.Core.Entities;

namespace RegulatoryComplianceApplication.Core.Interfaces
{
    public interface IBillService
    {
        Task<IEnumerable<Bill>> GetAllAsync();
        Task<IEnumerable<Bill>> GetDueSoonAsync(int days);

        Task<IEnumerable<Bill>> GetOverdueAsync();
        Task<Bill?> GetByIdAsync(int billId);
        Task<Bill> CreateAsync(Bill bill, int createdByUserId);
        Task UpdateAsync(Bill bill, int editedByUserId);
        Task SoftDeleteAsync(int billId, int deletedByUserId);
    }
}