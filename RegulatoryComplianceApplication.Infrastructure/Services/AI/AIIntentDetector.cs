using Microsoft.Extensions.Configuration;
using RegulatoryComplianceApplication.Core.Interfaces;
using RegulatoryComplianceApplication.Core.Models;
using System.Text;
using System.Text.Json;

namespace RegulatoryComplianceApplication.Infrastructure.Services.AI
{
    public class AIIntentDetector : IAIIntentDetector
    {
        private readonly IConfiguration _configuration;

        public AIIntentDetector(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<AssistantIntent> DetectIntentAsync(string question)
        {
            try
            {
                using var client = new HttpClient();

                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue(
                        "Bearer",
                        _configuration["Mistral:ApiKey"]);

                var request = new
                {
                    model = _configuration["Mistral:Model"],
                    messages = new[]
                    {
                    new
                    {
                        role = "system",
                        content =
                        """
                        You are an intent classifier.

                        Return ONLY ONE of the following intent names.

                        GeneralKnowledge
                        Documents
                        DocumentsByUser
                        ExpiringDocuments
                        Bills
                        OverdueBills
                        DueSoonBills
                        Users
                        Notifications
                        AuditLogs
                        Dashboard

                        Rules:

                        - Questions about documents owned by or assigned to a user → DocumentsByUser.
                        - Questions about bills, payments, invoices, or bill ownership → Bills.
                        - Questions about overdue bills → OverdueBills.
                        - Questions about bills due soon or upcoming bills → DueSoonBills.
                        - Questions asking for all documents → Documents.
                        - Questions asking for all bills → Bills.
                        - General regulatory or compliance knowledge → GeneralKnowledge.

                        Only output the intent name.
                        Do not explain.
                        Do not use punctuation.
                        """
                    },
                    new
                    {
                        role = "user",
                        content = question
                    }
                }
                };

                var json = JsonSerializer.Serialize(request);

                var response = await client.PostAsync(
                    "https://api.mistral.ai/v1/chat/completions",
                    new StringContent(json, Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode)
                {

                    if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {

                        return AssistantIntent.GeneralKnowledge;
                    }
                    return AssistantIntent.GeneralKnowledge;
                }

                var responseJson = await response.Content.ReadAsStringAsync();

                using var document = JsonDocument.Parse(responseJson);

                var intent = document.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString()!
                    .Trim();

                return Enum.TryParse<AssistantIntent>(intent, true, out var result)
                    ? result
                    : AssistantIntent.GeneralKnowledge;
            }
            catch
            {
                return AssistantIntent.GeneralKnowledge;
            }
        }
    }
}