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
            var apiKey = _configuration["Mistral:ApiKey"];
            var model = _configuration["Mistral:Model"];

            using var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);


            var prompt =
                 $"""
                You are the AI Assistant for the Regulatory Compliance Management System.

                You have TWO responsibilities.

                --------------------------------------------------------

                1. GENERAL KNOWLEDGE

                You are allowed to answer questions about:

                • Regulatory Compliance
                • Saudi Arabian regulations
                • ISO standards
                • Compliance best practices
                • Risk management
                • Auditing
                • Bills
                • Document management
                • Business compliance
                • Government compliance portals
                • Renewal procedures
                • Regulatory terminology

                When answering these questions, use your own knowledge.

                If a regulation may change over time, advise the user to verify the latest requirements with the appropriate Saudi authority.

                --------------------------------------------------------

                2. APPLICATION DATA

                If application data is provided below, use it as the authoritative source for questions about this application. Do not invent additional application data.

                • Documents
                • Bills
                • Users
                • Notifications
                • Audit Logs
                • Dashboard statistics

                Never invent application data.

                If the requested application information is not provided, clearly say that it is unavailable.

                --------------------------------------------------------
                Do NOT answer general knowledge (for example baking a cake, driving a car, programming, etc). Your purpose is to ONLY assist with the applicationa and that is it. If the user asks about such things outside the scope of what you are meant to do, simply reply with: This is outside the scope of my expertise, I could help you instead with (Insert some suggestions for prompts).

                APPLICATION DATA

                {context}

                --------------------------------------------------------

                USER QUESTION

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

            try
            {
                var response = await client.PostAsync(
                    "https://api.mistral.ai/v1/chat/completions",
                    new StringContent(json, Encoding.UTF8, "application/json"));

                var responseJson = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        return """
The AI assistant is temporarily unavailable because the AI service has reached its request or capacity limit.

Please try again in a few minutes.
""";
                    }

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        return """
The AI assistant could not authenticate with the AI service.

Please contact the system administrator.
""";
                    }

                    return """
The AI service returned an unexpected error.

Please try again later.
""";
                }

                using var document = JsonDocument.Parse(responseJson);

                return document.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString() ?? "No response.";
            }
            catch (HttpRequestException)
            {
                return """
Unable to connect to the AI service.

Please check your internet connection and try again.
""";
            }
            catch (TaskCanceledException)
            {
                return """
The AI service took too long to respond.

Please try again.
""";
            }
            catch
            {
                return """
An unexpected error occurred while contacting the AI assistant.

Please try again later.
""";
            }
        }
    }
}