

namespace NPAG.Models
{
    public class PayRequest
    {

        public string OrderNumber { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string SecurityCode { get; set; }

        public string PaymentDate { get; set; }
        public decimal PaymentAmount { get; set; }
        public string UserID { get; set; }
        public string ReceiptNumber { get; set; }
        public string PaymentMode { get; set; }
    }
}
