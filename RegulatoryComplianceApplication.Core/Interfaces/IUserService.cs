using RegulatoryComplianceApplication.Core.Entities;

namespace RegulatoryComplianceApplication.Core.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetByIdAsync(int userId);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> CreateAsync(User user, string plainPassword);
    }
}