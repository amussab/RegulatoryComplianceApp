namespace RegulatoryComplianceApplication.Core.Models
{
    public class ComplianceAnalyticsResult
    {
        public double OverallScore { get; set; }

        public string Grade { get; set; } = string.Empty;

        public string RiskLevel { get; set; } = string.Empty;

        public double FinancialHealth { get; set; }

        public double ComplianceHealth { get; set; }

        public decimal OutstandingAmount { get; set; }

        public List<string> Recommendations { get; set; } = new();
    }
}