namespace RegulatoryComplianceApplication.Core.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string recipientEmail, string subject, string body);
    }
}