using Microsoft.EntityFrameworkCore;
using RegulatoryComplianceApplication.Core.Entities;
using RegulatoryComplianceApplication.Core.Interfaces;
using RegulatoryComplianceApplication.Infrastructure.Data;

namespace RegulatoryComplianceApplication.Infrastructure.Services
{
    public class BillService : IBillService
    {
        private readonly AppDbContext _context;

        public BillService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Bill>> GetAllAsync()
        {
            return await _context.Bills
                .Include(b => b.User)
                .Where(b => !b.IsDeleted)
                .OrderBy(b => b.DueDate)
                .ToListAsync();
        }

        public async Task<Bill?> GetByIdAsync(int billId)
        {
            return await _context.Bills
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.BillId == billId);
        }

        public async Task<Bill> CreateAsync(Bill bill)
        {
            bill.CreatedAt = DateTime.UtcNow;

            _context.Bills.Add(bill);

            await _context.SaveChangesAsync();

            return bill;
        }

        public async Task UpdateAsync(Bill bill)
        {
            _context.Bills.Update(bill);

            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int billId)
        {
            var bill = await _context.Bills.FindAsync(billId);

            if (bill == null)
                return;

            bill.IsDeleted = true;

            await _context.SaveChangesAsync();
        }
    }
}