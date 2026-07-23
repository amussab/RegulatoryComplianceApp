using RegulatoryComplianceApplication.Core.Models;

namespace RegulatoryComplianceApplication.Core.Interfaces
{
    public interface IAIIntentDetector
    {
        Task<AssistantIntent> DetectIntentAsync(string question);
    }
}