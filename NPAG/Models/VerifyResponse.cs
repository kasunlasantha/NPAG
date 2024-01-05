using NPAG.Entities;

namespace NPAG.Models
{
    public class VerifyResponse
    {
        public string CustomerName { get; set; }
        public decimal? ContactNumber { get; set; }
        public string BalanceAmount { get; set; }

        //public string CustomerAddress { get; set; }
        public string JwtToken { get; set; }

        public VerifyResponse(CRM_INVOICES compdata,string jwtToken)
        {
            JwtToken = jwtToken;
            CustomerName = compdata.BILLING_ACC_NAME;
            ContactNumber = compdata.CONTACT_NO;
            BalanceAmount = compdata.BALANCE_AMOUNT;
            //CustomerAddress = (compdata.ADDRESSLINE01 +","+ compdata.ADDRESSLINE02 + "," + compdata.ADDRESSLINE03 + "," + compdata.ADDRESSLINE04);
        }
    }
}
