using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegulatoryComplianceApplication.Core.Entities;
using RegulatoryComplianceApplication.Infrastructure.Data;

namespace RegulatoryComplianceApplication.Web.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class DocumentTypesController : Controller
    {
        private readonly AppDbContext _context;

        public DocumentTypesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var types = await _context.DocumentTypes
                .Where(t => !t.IsDeleted)
                .OrderBy(t => t.TypeName)
                .ToListAsync();

            return View(types);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DocumentType model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _context.DocumentTypes.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var type = await _context.DocumentTypes.FindAsync(id);

            if (type == null || type.IsDeleted)
                return NotFound();

            return View(type);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DocumentType model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var type = await _context.DocumentTypes.FindAsync(model.DocumentTypeId);

            if (type == null)
                return NotFound();

            type.TypeName = model.TypeName;
            type.IsExpirable = model.IsExpirable;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var type = await _context.DocumentTypes.FindAsync(id);

            if (type == null)
                return NotFound();

            return View(type);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var type = await _context.DocumentTypes.FindAsync(id);

            if (type == null)
                return NotFound();

            type.IsDeleted = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}