using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegulatoryComplianceApplication.Core.Interfaces;
using RegulatoryComplianceApplication.Infrastructure.Data;
using RegulatoryComplianceApplication.Infrastructure.Services.AI;
using RegulatoryComplianceApplication.ViewModels.Assistant;


namespace RegulatoryComplianceApplication.Controllers
{
    [Authorize]
    public class AssistantController : Controller
    {
        private readonly IAIService _aiService;
        private readonly IAIIntentDetector _intentDetector;
        private readonly IAIContextBuilder _contextBuilder;


        public AssistantController(
            IAIService aiService,
            IAIIntentDetector intentDetector,
            IAIContextBuilder contextBuilder,
            IDocumentService documentService,
            IBillService billService,
            AppDbContext context)
        {
            _aiService = aiService;
            _intentDetector = intentDetector;
            _contextBuilder = contextBuilder;
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

            var intent = await _intentDetector.DetectIntentAsync(model.Question);


            var context = await _contextBuilder.BuildContextAsync(
                intent,
                model.Question);

            model.Response = await _aiService.AskAsync(
                model.Question,
                context);

            return View(model);
        }
    }
}