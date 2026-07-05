using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RegulatoryComplianceApplication.Core.Entities;
using RegulatoryComplianceApplication.Core.Interfaces;
using RegulatoryComplianceApplication.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace RegulatoryComplianceApplication.Web.Controllers
{
    public class DocumentsController : Controller
    {
        private readonly IDocumentService _documentService;
        private readonly IFileStorageService _fileStorageService;
        private readonly RegulatoryComplianceApplication.Infrastructure.Data.AppDbContext _context;
        public DocumentsController(IDocumentService documentService, IFileStorageService fileStorageService, RegulatoryComplianceApplication.Infrastructure.Data.AppDbContext context)
        {
            _documentService = documentService;
            _fileStorageService = fileStorageService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var documents = await _documentService.GetAllAsync();

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var cutoff = today.AddDays(60);

            var viewModels = documents.Select(d =>
            {
                var expiry = d.CurrentVersion?.ExpiryDate;
                string status = expiry == null ? "Unknown"
                    : expiry < today ? "Expired"
                    : expiry <= cutoff ? "Expiring Soon"
                    : "Valid";

                return new DocumentListViewModel
                {
                    DocumentId = d.DocumentId,
                    Title = d.Title,
                    DocumentNumber = d.DocumentNumber,
                    ExpiryDate = expiry,
                    Status = status
                };
            }).ToList();

            return View(viewModels);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Renew(int id, DateOnly newExpiryDate, IFormFile file)
        {
            const int hardcodedUserId = 1;
            var filePath = await _fileStorageService.SaveFileAsync(file.OpenReadStream(), file.FileName);

            var newVersion = new DocumentVersion
            {
                FilePath = filePath,
                IssueDate = DateOnly.FromDateTime(DateTime.UtcNow),
                ExpiryDate = newExpiryDate
            };

            await _documentService.RenewAsync(id, newVersion, hardcodedUserId);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Create()
        {
            var vm = new CreateDocumentViewModel
            {
                DocumentTypes = await _context.DocumentTypes
                    .Select(t => new SelectListItem { Value = t.DocumentTypeId.ToString(), Text = t.TypeName })
                    .ToListAsync()
            };
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDocumentViewModel vm)
        {
            if (vm.File == null || vm.File.Length == 0)
                ModelState.AddModelError("File", "A file is required.");

            if (!ModelState.IsValid)
            {
                vm.DocumentTypes = await _context.DocumentTypes
                    .Select(t => new SelectListItem { Value = t.DocumentTypeId.ToString(), Text = t.TypeName })
                    .ToListAsync();
                return View(vm);
            }

            var filePath = await _fileStorageService.SaveFileAsync(vm.File!.OpenReadStream(), vm.File.FileName);

            var document = new Document
            {
                DocumentTypeId = vm.DocumentTypeId,
                Title = vm.Title,
                DocumentNumber = vm.DocumentNumber,
                CreatedAt = DateTime.UtcNow
            };

            const int hardcodedUserId = 1; // TEMP: no auth yet, replace with logged-in user ID once auth exists

            var firstVersion = new DocumentVersion
            {
                FilePath = filePath,
                IssueDate = vm.IssueDate,
                ExpiryDate = vm.ExpiryDate,
                UploadedByUserId = hardcodedUserId
            };

            await _documentService.CreateAsync(document, firstVersion, hardcodedUserId);

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Details(int id)
        {
            var document = await _documentService.GetByIdAsync(id);
            if (document == null) return NotFound();

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var cutoff = today.AddDays(60);
            var expiry = document.CurrentVersion?.ExpiryDate;
            string status = expiry == null ? "Unknown"
                : expiry < today ? "Expired"
                : expiry <= cutoff ? "Expiring Soon"
                : "Valid";

            var vm = new DocumentDetailsViewModel
            {
                DocumentId = document.DocumentId,
                Title = document.Title,
                DocumentNumber = document.DocumentNumber,
                Status = status,
                Versions = document.Versions
                    .OrderByDescending(v => v.VersionNumber)
                    .Select(v => new VersionRow
                    {
                        VersionNumber = v.VersionNumber,
                        IssueDate = v.IssueDate,
                        ExpiryDate = v.ExpiryDate,
                        FilePath = v.FilePath,
                        IsCurrent = v.DocumentVersionId == document.CurrentVersionId
                    }).ToList()
            };

            return View(vm);
        }

        public async Task<IActionResult> Download(string filePath)
        {
            var stream = await _fileStorageService.GetFileAsync(filePath);
            return File(stream, "application/octet-stream", Path.GetFileName(filePath));
        }
        public async Task<IActionResult> Dashboard()
        {
            var documents = await _documentService.GetAllAsync();
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var cutoff = today.AddDays(60);

            var vm = new DashboardViewModel();
            foreach (var d in documents)
            {
                var expiry = d.CurrentVersion?.ExpiryDate;
                if (expiry == null) continue;
                if (expiry < today) vm.ExpiredCount++;
                else if (expiry <= cutoff) vm.ExpiringSoonCount++;
                else vm.ValidCount++;
            }

            return View(vm);
        }
    }
}