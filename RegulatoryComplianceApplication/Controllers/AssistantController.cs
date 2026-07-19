using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegulatoryComplianceApplication.Infrastructure.Services.AI;
using RegulatoryComplianceApplication.ViewModels.Assistant;

namespace RegulatoryComplianceApplication.Controllers
{
    [Authorize]
    public class AssistantController : Controller
    {
        private readonly IAIService _aiService;

        public AssistantController(IAIService aiService)
        {
            _aiService = aiService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new ChatViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(ChatViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Temporary context
            string context = """
            This is a Regulatory Compliance Management System.

            It manages:
            - Documents
            - Bills
            - Users
            - Notifications
            """;

            model.Response = await _aiService.AskAsync(model.Question, context);

            return View(model);
        }
    }
}