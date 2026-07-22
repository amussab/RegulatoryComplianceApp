using RegulatoryComplianceApplication.Core.Models;

namespace RegulatoryComplianceApplication.Web.ViewModels
{
    public class DashboardViewModel
    {
        // Documents
        public int ValidCount { get; set; }

        public int ExpiringSoonCount { get; set; }

        public int ExpiredCount { get; set; }

        // Bills
        public int PaidBills { get; set; }

        public int PendingBills { get; set; }

        public int OverdueBills { get; set; }

        public decimal OutstandingAmount { get; set; }
        public ComplianceAnalyticsResult Analytics { get; set; } = new();
    }
}