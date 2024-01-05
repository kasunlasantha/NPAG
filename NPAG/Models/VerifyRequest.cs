

using System.ComponentModel.DataAnnotations;

namespace NPAG.Models
{
    public class VerifyRequest
    {
        //[Required(ErrorMessage = "Error-6002: Invalid Order Number")]
        public string OrderNumber { get; set; }

        //[Required(ErrorMessage = "Error-6003: Invalid Company Code")]
        public decimal? CompanyCode { get; set; }

        //[Required(ErrorMessage = "Error-6004: Invalid Branch Code")]
        public decimal? BranchCode { get; set; }

       // [Required(ErrorMessage = "Error-6010: Invalid Security Code")]
        public string SecurityCode { get; set; }
        //public string JwtToken { get; set; }

        //public VerifyRequest()
        //{

        //}
    }
}
