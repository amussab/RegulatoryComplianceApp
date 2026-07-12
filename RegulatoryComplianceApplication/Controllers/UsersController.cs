using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegulatoryComplianceApplication.Core.Entities;
using RegulatoryComplianceApplication.Core.Interfaces;
using RegulatoryComplianceApplication.Infrastructure.Data;
using RegulatoryComplianceApplication.Web.ViewModels;

namespace RegulatoryComplianceApplication.Web.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly AppDbContext _context;

        public UsersController(IUserService userService, AppDbContext context)
        {
            _userService = userService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .OrderBy(u => u.FullName)
                .ToListAsync();

            return View(users);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                var user = new User
                {
                    FullName = vm.FullName,
                    Email = vm.Email,
                    RoleId = vm.RoleId,
                    IsActive = true
                };

                await _userService.CreateAsync(user, vm.Password);

                TempData["Success"] = "User created successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(vm);
            }
        }
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound();

            var vm = new EditUserViewModel
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                RoleId = user.RoleId,
                IsActive = user.IsActive
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var user = await _context.Users.FindAsync(vm.UserId);

            if (user == null)
                return NotFound();

            user.FullName = vm.FullName;
            user.Email = vm.Email;
            user.RoleId = vm.RoleId;
            user.IsActive = vm.IsActive;

            await _context.SaveChangesAsync();

            TempData["Success"] = "User updated successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}