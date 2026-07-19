namespace RegulatoryComplianceApplication.Infrastructure.Services.AI
{
    public interface IAIService
    {
        Task<string> AskAsync(string question, string context);
    }
}