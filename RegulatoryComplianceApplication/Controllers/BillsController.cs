using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RegulatoryComplianceApplication.Core.Entities;
using RegulatoryComplianceApplication.Core.Interfaces;
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

        public BillsController(
            IBillService billService,
            AppDbContext context,
            IFileStorageService fileStorageService)
        {
            _billService = billService;
            _context = context;
            _fileStorageService = fileStorageService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _billService.GetAllAsync());
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
        public async Task<IActionResult> Details(int id)
        {
            var bill = await _billService.GetByIdAsync(id);

            if (bill == null)
                return NotFound();

            return View(bill);
        }
    }
}