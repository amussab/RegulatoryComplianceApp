using System.Diagnostics;
using System.Text;
using System.Text.Json;
using RegulatoryComplianceApplication.Core.Entities;
using RegulatoryComplianceApplication.Core.Interfaces;
using RegulatoryComplianceApplication.Core.Models;

namespace RegulatoryComplianceApplication.Infrastructure.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        public async Task<ComplianceAnalyticsResult> AnalyzeAsync(
            IEnumerable<Document> documents,
            IEnumerable<Bill> bills)
        {
            var input = new
            {
                documents = documents.Select(d => new
                {
                    status =
                        !d.DocumentType.IsExpirable ? "No Expiry" :
                        d.CurrentVersion?.ExpiryDate == null ? "Unknown" :
                        d.CurrentVersion.ExpiryDate < DateOnly.FromDateTime(DateTime.UtcNow) ? "Expired" :
                        d.CurrentVersion.ExpiryDate <= DateOnly.FromDateTime(DateTime.UtcNow).AddDays(60) ? "Expiring Soon" :
                        "Valid"
                }),

                bills = bills.Select(b => new
                {
                    status = b.Status.ToString(),
                    amount = b.Amount
                })
            };

            var json = JsonSerializer.Serialize(input);

            var process = new Process();

            process.StartInfo.FileName = "python";
            process.StartInfo.Arguments = "Python/Analytics.py";

            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            await process.StandardInput.WriteAsync(json);
            process.StandardInput.Close();

            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            if (!string.IsNullOrWhiteSpace(error))
            {
                throw new Exception(error);
            }

            if (string.IsNullOrWhiteSpace(output))
            {
                throw new Exception("Python returned no output.");
            }

            return JsonSerializer.Deserialize<ComplianceAnalyticsResult>(output)!;
        }
    }
}