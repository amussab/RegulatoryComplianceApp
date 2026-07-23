using RegulatoryComplianceApplication.Core.Models;

namespace RegulatoryComplianceApplication.Core.Interfaces
{
    public interface IAIContextBuilder
    {
        Task<string> BuildContextAsync(
            AssistantIntent intent,
            string question);
    }
}