using RegulatoryComplianceApplication.Core.Entities;

namespace RegulatoryComplianceApplication.Core.Interfaces
{
    public interface IBillService
    {
        Task<IEnumerable<Bill>> GetAllAsync();
        Task<IEnumerable<Bill>> GetDueSoonAsync(int days);

        Task<IEnumerable<Bill>> GetOverdueAsync();
        Task<Bill?> GetByIdAsync(int billId);
        Task<Bill> CreateAsync(Bill bill);
        Task UpdateAsync(Bill bill);
        Task SoftDeleteAsync(int billId);
    }
}