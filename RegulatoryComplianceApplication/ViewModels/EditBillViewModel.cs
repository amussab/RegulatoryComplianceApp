using RegulatoryComplianceApplication.Core.Entities;

namespace RegulatoryComplianceApplication.Web.ViewModels
{
    public class EditBillViewModel : CreateBillViewModel
    {
        public int BillId { get; set; }

        public string? CurrentAttachmentPath { get; set; }
    }
}