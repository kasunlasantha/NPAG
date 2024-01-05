
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace NPAG.Entities
{
    [Table("NPAG_ERROR_PAYMENT")]
    public class NPAG_ERROR_PAYMENT
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PAYMENTID { get; set; }

        [StringLength(50)]
        public string ORDER_NUMBER { get; set; }

        [StringLength(50)]
        public string COMPANYCODE { get; set; }

        [StringLength(50)]
        public string BRANCHCODE { get; set; }

        [StringLength(50)]
        public string AMOUNT { get; set; }

        [StringLength(50)]
        public string PAYMENTDATE { get; set; }

        [StringLength(50)]
        public string USERID { get; set; }

        [StringLength(50)]
        public string SECURECODE { get; set; }

        [StringLength(50)]
        public string RECEIPTNUMBER { get; set; }

        [StringLength(200)]
        public string ERRORMESSAGE { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string CREATE_DATE { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string LAST_UPDATED_DATE { get; set; }

        [StringLength(20)]
        public string IP_ADDRESS { get; set; }

    }
}
