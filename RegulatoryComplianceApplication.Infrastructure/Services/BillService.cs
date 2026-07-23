using Microsoft.EntityFrameworkCore;
using RegulatoryComplianceApplication.Core.Entities;
using RegulatoryComplianceApplication.Core.Interfaces;
using RegulatoryComplianceApplication.Infrastructure.Data;

namespace RegulatoryComplianceApplication.Infrastructure.Services
{
    public class BillService : IBillService
    {
        private readonly AppDbContext _context;
        private readonly IAuditLogger _auditLogger;

        public BillService(AppDbContext context, IAuditLogger auditLogger)
        {
            _context = context;
            _auditLogger = auditLogger;
        }

        public async Task<IEnumerable<Bill>> GetAllAsync()
        {
            return await _context.Bills
                .Include(b => b.User)
                .Where(b => !b.IsDeleted)
                .OrderBy(b => b.DueDate)
                .ToListAsync();
        }
        public async Task<IEnumerable<Bill>> GetDueSoonAsync(int days)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var cutoff = today.AddDays(days);

            return await _context.Bills
                .Include(b => b.User)
                .Where(b =>
                    !b.IsDeleted &&
                    b.Status != BillStatus.Paid &&
                    b.DueDate >= today &&
                    b.DueDate <= cutoff)
                .OrderBy(b => b.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Bill>> GetOverdueAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            return await _context.Bills
                .Include(b => b.User)
                .Where(b =>
                    !b.IsDeleted &&
                    b.Status != BillStatus.Paid &&
                    b.DueDate < today)
                .OrderBy(b => b.DueDate)
                .ToListAsync();
        }

        public async Task<Bill?> GetByIdAsync(int billId)
        {
            return await _context.Bills
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.BillId == billId);
        }

        public async Task<Bill> CreateAsync(Bill bill, int createdByUserId)
        {
            bill.CreatedAt = DateTime.UtcNow;

            _context.Bills.Add(bill);

            await _context.SaveChangesAsync();

            await _auditLogger.LogAsync(
                createdByUserId,
                "Create",
                "Bill",
                bill.BillId,
                "Bill",
                null,
                $"{bill.BillName} ({bill.Amount:C})");

            return bill;
        }

        public async Task UpdateAsync(Bill bill, int editedByUserId)
        {
            var existingBill = await _context.Bills
            .FirstOrDefaultAsync(b => b.BillId == bill.BillId);

            if (existingBill == null)
                throw new InvalidOperationException("Bill not found.");

            var originalBill = new Bill
            {
                BillName = existingBill.BillName,
                Amount = existingBill.Amount,
                DueDate = existingBill.DueDate,
                Frequency = existingBill.Frequency,
                Status = existingBill.Status,
                UserId = existingBill.UserId,
                IsRecurring = existingBill.IsRecurring,
                AttachmentPath = existingBill.AttachmentPath
            };

            existingBill.BillName = bill.BillName;
            existingBill.Amount = bill.Amount;
            existingBill.DueDate = bill.DueDate;
            existingBill.Frequency = bill.Frequency;
            existingBill.Status = bill.Status;
            existingBill.UserId = bill.UserId;
            existingBill.IsRecurring = bill.IsRecurring;
            existingBill.AttachmentPath = bill.AttachmentPath;

            await _context.SaveChangesAsync();

            await _auditLogger.LogChangesAsync(
                editedByUserId,
                "Edit",
                "Bill",
                existingBill.BillId,
                originalBill,
                existingBill,
                nameof(Bill.BillName),
                nameof(Bill.Amount),
                nameof(Bill.DueDate),
                nameof(Bill.Frequency),
                nameof(Bill.Status),
                nameof(Bill.UserId),
                nameof(Bill.IsRecurring),
                nameof(Bill.AttachmentPath));
        }

        public async Task SoftDeleteAsync(int billId, int deletedByUserId)
        {
            var bill = await _context.Bills.FindAsync(billId);

            if (bill == null)
                return;

            bill.IsDeleted = true;

            await _context.SaveChangesAsync();
            await _auditLogger.LogAsync(
                deletedByUserId,
                "Delete",
                "Bill",
                bill.BillId,
                "Bill",
                $"{bill.BillName} ({bill.Amount:C})",
                "Soft Deleted");
        }
    }
}