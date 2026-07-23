using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RegulatoryComplianceApplication.Core.Entities;
using RegulatoryComplianceApplication.Core.Interfaces;
using RegulatoryComplianceApplication.Infrastructure.Data;

namespace RegulatoryComplianceApplication.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IAuditLogger _auditLogger;
        private readonly PasswordHasher<User> _hasher = new();

        public UserService(AppDbContext context, IAuditLogger auditLogger)
        {
            _context = context;
            _auditLogger = auditLogger;
        }

        public async Task<User?> GetByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> CreateAsync(User user, string plainPassword, int createdByUserId)
        {
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                throw new InvalidOperationException("Email already in use.");

            user.PasswordHash = _hasher.HashPassword(user, plainPassword);
            user.CreatedAt = DateTime.UtcNow;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await _auditLogger.LogAsync(
                createdByUserId,
                "Create",
                "User",
                user.UserId,
                "User",
                null,
                $"{user.FullName} ({user.Email})");

            return user;
        }
        public async Task UpdateAsync(User user, int editedByUserId)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == user.UserId);

            if (existingUser == null)
                throw new InvalidOperationException("User not found.");

            var originalUser = new User
            {
                FullName = existingUser.FullName,
                Email = existingUser.Email,
                RoleId = existingUser.RoleId,
                IsActive = existingUser.IsActive
            };

            existingUser.FullName = user.FullName;
            existingUser.Email = user.Email;
            existingUser.RoleId = user.RoleId;
            existingUser.IsActive = user.IsActive;

            await _context.SaveChangesAsync();

            await _auditLogger.LogChangesAsync(
                editedByUserId,
                "Edit",
                "User",
                user.UserId,
                originalUser,
                existingUser,
                nameof(User.FullName),
                nameof(User.Email),
                nameof(User.RoleId),
                nameof(User.IsActive));
        }
        public async Task<User?> ValidateCredentialsAsync(string email, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

            if (user == null)
                return null;

            var result = _hasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                password);

            return result == PasswordVerificationResult.Success
                ? user
                : null;
        }
    }
}