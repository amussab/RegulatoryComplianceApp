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

Your job is to return ONLY ONE of these words.

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

Do not explain.

Do not write punctuation.

Only output ONE word from the list.
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