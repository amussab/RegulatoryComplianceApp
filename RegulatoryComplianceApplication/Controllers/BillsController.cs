using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RegulatoryComplianceApplication.Core.Entities;
using RegulatoryComplianceApplication.Core.Interfaces;
using RegulatoryComplianceApplication.Core.Models;
using RegulatoryComplianceApplication.Infrastructure.Data;
using RegulatoryComplianceApplication.Web.ViewModels;

namespace RegulatoryComplianceApplication.Web.Controllers
{
    [Authorize]
    public class BillsController : Controller
    {
        private readonly IBillService _billService;
        private readonly AppDbContext _context;
        private readonly IFileStorageService _fileStorageService;
        private readonly IReportService _reportService;
        public BillsController(
            IBillService billService,
            AppDbContext context,
            IFileStorageService fileStorageService,
            IReportService reportService)
        {
            _billService = billService;
            _context = context;
            _fileStorageService = fileStorageService;
            _reportService = reportService;
        }

        public async Task<IActionResult> Index(
            string? search,
            BillStatus? status,
            BillFrequency? frequency,
            int? userId,
            string? dueFilter)
        {
            var bills = await _billService.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                bills = bills.Where(b =>
                    b.BillName.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            if (status.HasValue)
            {
                bills = bills.Where(b => b.Status == status.Value);
            }
            if (frequency.HasValue)
            {
                bills = bills.Where(b => b.Frequency == frequency.Value);
            }

            if (userId.HasValue)
            {
                bills = bills.Where(b => b.UserId == userId.Value);
            }
            var today = DateOnly.FromDateTime(DateTime.Today);

            switch (dueFilter)
            {
                case "Today":
                    bills = bills.Where(b => b.DueDate == today);
                    break;

                case "Next7":
                    bills = bills.Where(b =>
                        b.DueDate >= today &&
                        b.DueDate <= today.AddDays(7));
                    break;

                case "Next30":
                    bills = bills.Where(b =>
                        b.DueDate >= today &&
                        b.DueDate <= today.AddDays(30));
                    break;

                case "Overdue":
                    bills = bills.Where(b => b.DueDate < today);
                    break;
            }

            ViewBag.Search = search;
            ViewBag.Status = status;
            ViewBag.Frequency = frequency;
            ViewBag.UserId = userId;
            ViewBag.DueFilter = dueFilter;

            ViewBag.Users = await _context.Users
                .Where(u => u.IsActive)
                .OrderBy(u => u.FullName)
                .ToListAsync();

            return View(bills);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create()
        {
            var vm = new CreateBillViewModel
            {
                DueDate = DateOnly.FromDateTime(DateTime.Today),

                Users = await _context.Users
                    .Where(u => u.IsActive)
                    .OrderBy(u => u.FullName)
                    .Select(u => new SelectListItem
                    {
                        Value = u.UserId.ToString(),
                        Text = u.FullName
                    })
                    .ToListAsync()
            };

            return View(vm);
        }
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id)
        {
            var bill = await _billService.GetByIdAsync(id);

            if (bill == null)
                return NotFound();

            var vm = new EditBillViewModel
            {
                BillId = bill.BillId,
                BillName = bill.BillName,
                Amount = bill.Amount,
                DueDate = bill.DueDate,
                Frequency = bill.Frequency,
                IsRecurring = bill.IsRecurring,
                Status = bill.Status,
                UserId = bill.UserId,
                CurrentAttachmentPath = bill.AttachmentPath,

                Users = await _context.Users
                    .Where(u => u.IsActive)
                    .OrderBy(u => u.FullName)
                    .Select(u => new SelectListItem
                    {
                        Value = u.UserId.ToString(),
                        Text = u.FullName
                    })
                    .ToListAsync()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(EditBillViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Users = await _context.Users
                    .Where(u => u.IsActive)
                    .OrderBy(u => u.FullName)
                    .Select(u => new SelectListItem
                    {
                        Value = u.UserId.ToString(),
                        Text = u.FullName
                    })
                    .ToListAsync();

                return View(vm);
            }

            var bill = await _billService.GetByIdAsync(vm.BillId);

            if (bill == null)
                return NotFound();

            if (vm.Attachment != null)
            {
                if (!string.IsNullOrEmpty(bill.AttachmentPath))
                    _fileStorageService.DeleteFile(bill.AttachmentPath);

                bill.AttachmentPath = await _fileStorageService.SaveFileAsync(
                    vm.Attachment.OpenReadStream(),
                    vm.Attachment.FileName);
            }

            bill.BillName = vm.BillName;
            bill.Amount = vm.Amount;
            bill.DueDate = vm.DueDate;
            bill.Frequency = vm.Frequency;
            bill.IsRecurring = vm.IsRecurring;
            bill.Status = vm.Status;
            bill.UserId = vm.UserId;

            await _billService.UpdateAsync(bill);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create(CreateBillViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Users = await _context.Users
                    .Where(u => u.IsActive)
                    .OrderBy(u => u.FullName)
                    .Select(u => new SelectListItem
                    {
                        Value = u.UserId.ToString(),
                        Text = u.FullName
                    })
                    .ToListAsync();

                return View(vm);
            }

            string? attachmentPath = null;

            if (vm.Attachment != null)
            {
                attachmentPath = await _fileStorageService.SaveFileAsync(
                    vm.Attachment.OpenReadStream(),
                    vm.Attachment.FileName);
            }

            var bill = new Bill
            {
                UserId = vm.UserId,
                BillName = vm.BillName,
                Amount = vm.Amount,
                DueDate = vm.DueDate,
                Frequency = vm.Frequency,
                IsRecurring = vm.IsRecurring,
                Status = vm.Status,
                AttachmentPath = attachmentPath
            };

            await _billService.CreateAsync(bill);

            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            var bill = await _billService.GetByIdAsync(id);

            if (bill == null)
                return NotFound();

            return View(bill);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _billService.SoftDeleteAsync(id);

            return RedirectToAction(nameof(Index));
        }
        [Authorize]
        public async Task<IActionResult> ExportBillsPdf()
        {
            var bills = await _billService.GetAllAsync();

            var reportData = bills.Select(b => new BillReportRow
            {
                BillName = b.BillName,
                Amount = b.Amount,
                AssignedUser = b.User.FullName,
                DueDate = b.DueDate,
                Frequency = b.Frequency.ToString(),
                Status = b.Status.ToString()
            }).ToList();

            int paidCount = reportData.Count(b => b.Status == "Paid");
            int pendingCount = reportData.Count(b => b.Status == "Pending");
            int overdueCount = reportData.Count(b => b.Status == "Overdue");

            var pdf = await _reportService.GenerateBillsPdfAsync(
                reportData,
                paidCount,
                pendingCount,
                overdueCount);

            return File(
                pdf,
                "application/pdf",
                $"BillsReport_{DateTime.Now:yyyyMMdd}.pdf");
        }
        public async Task<IActionResult> Details(int id)
        {
            var bill = await _billService.GetByIdAsync(id);

            if (bill == null)
                return NotFound();

            return View(bill);
        }
        [Authorize]
        public async Task<IActionResult> ExportBillsExcel()
        {
            var bills = await _billService.GetAllAsync();

            var reportData = bills.Select(b => new BillReportRow
            {
                BillName = b.BillName,
                Amount = b.Amount,
                AssignedUser = b.User.FullName,
                DueDate = b.DueDate,
                Frequency = b.Frequency.ToString(),
                Status = b.Status.ToString()
            }).ToList();

            var excel = await _reportService.GenerateBillsExcelAsync(reportData);

            return File(
                excel,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"BillsReport_{DateTime.Now:yyyyMMdd}.xlsx");
        }
    }
}