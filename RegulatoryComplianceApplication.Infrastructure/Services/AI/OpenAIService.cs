using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace RegulatoryComplianceApplication.Infrastructure.Services.AI
{
    public class OpenAIService : IAIService
    {
        private readonly IConfiguration _configuration;

        public OpenAIService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> AskAsync(string question, string context)
        {
            var apiKey = _configuration["OpenRouter:ApiKey"];
            var model = _configuration["OpenRouter:Model"];

            using var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

            client.DefaultRequestHeaders.Add("HTTP-Referer", "http://localhost");
            client.DefaultRequestHeaders.Add("X-Title", "Regulatory Compliance System");

            var prompt =
$"""
You are an AI Compliance Assistant.

Answer ONLY using the information provided below.

If the answer cannot be found in the context, reply:

"I couldn't find that information in the system."

Context:
{context}

User Question:
{question}
""";

            var request = new
            {
                model = model,
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = prompt
                    }
                }
            };

            var json = JsonSerializer.Serialize(request);

            var response = await client.PostAsync(
                "https://openrouter.ai/api/v1/chat/completions",
                new StringContent(json, Encoding.UTF8, "application/json"));

            var responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception(responseJson);

            using var document = JsonDocument.Parse(responseJson);

            return document.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "No response.";
        }
    }
}