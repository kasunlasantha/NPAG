using System;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace NPAG.Entities
{
    [Table("CRM_PAYMENT")]
    public class CRM_PAYMENT
    {
        public DateTime? PAYMENTDATE { get; set; }

        [StringLength(5)]
        public string BANKCODE { get; set; }

        [StringLength(5)]
        public string BRANCHCODE { get; set; }

        [StringLength(2)]
        public string PAYMENTMODE { get; set; }

        [StringLength(10)]
        public string CASHIER { get; set; }

        public decimal? AMOUNT { get; set; }

        public string PAYMENTREF { get; set; }

        public string CURRENCY { get; set; }

        public decimal? PAYSEQ_OUT { get; set; }

        public string ITEMACC_OUT { get; set; }

        public decimal? ACCPAYSEQ_OUT { get; set; }

        public string RECEIPTNUMBER { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal? CRMPAYMENTID { get; set; }

        public string BILLING_ACCOUNTNUMBER { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime? CREATEDATE { get; set; }

        public string CENTERCODE { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ERRORCODE { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal? STATUS { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal? TRYCOUNT { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal? CANCELSTATUS { get; set; }

        public decimal? PAYMENTIDORIGIN { get; set; }

        public string ACCOUNTNO_OUT { get; set; }

        public string BILLING_TYPE { get; set; }

        public string ORDER_NUMBER { get; set; }

        public string CUSTOMERREF { get; set; }

        public string PAYMENT_TEXT { get; set; }

        public string ORDER_ID { get; set; }

        public string CRM_RETURNMESSAGE { get; set; }

        public string CRM_RETURNCODE { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime? LASTUPDATEDTIME { get; set; }

        public decimal? CANCELLATION_REASON { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal? CRM_STATUS { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal? CRM_TRYCOUNT { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal? EDT_SEND { get; set; }

    }
}
