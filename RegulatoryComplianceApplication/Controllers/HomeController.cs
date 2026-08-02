using Microsoft.AspNetCore.Mvc;
using RegulatoryComplianceApplication.Models;
using System.Diagnostics;
using RegulatoryComplianceApplication.Core.Interfaces;

namespace RegulatoryComplianceApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmailService _emailService;

        public HomeController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public async Task<IActionResult> TestEmail()
        {
            await _emailService.SendEmailAsync(
                "abdulrazaqmussab1@gmail.com",
                "SMTP Test",
                "Email");

            return Content("Email sent successfully!");
        }
    }
}
