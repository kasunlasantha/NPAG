using System;
using System.ComponentModel.DataAnnotations.Schema;


namespace NPAG.Entities
{
    [Table("CRM_INVOICES")]
    public class CRM_INVOICES
    {
        public decimal? CRM_GETID { get; set; }
        public string REVISION_NO { get; set; }
        public string PERFORMA_INVOICE_NO { get; set; }
        public string CUS_ACC_NO { get; set; }
        public string BILLING_ACC_NO { get; set; }
        public string BILLING_ACC_TYPE { get; set; }
        public string ORDER_NO { get; set; }
        public string ORDER_ID { get; set; }
        public string ORDER_ITEM_ID { get; set; }
        public string BILLING_ACC_NAME { get; set; }
        public decimal? CONTACT_NO { get; set; }
        public string BALANCE_AMOUNT { get; set; }
        public string ORDER_STATUS { get; set; }
        public string COUNTRY { get; set; }
        public string PROVINCE { get; set; }
        public string DISTRICT { get; set; }
        public string CITY { get; set; }
        public string POSTEL_CODE { get; set; }
        public string ADDRESSLINE01 { get; set; }
        public string ADDRESSLINE02 { get; set; }
        public string ADDRESSLINE03 { get; set; }
        public string ADDRESSLINE04 { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public DateTime? LAST_UPDATED_DATE { get; set; }
        public string STATUS { get; set; }
        public DateTime? INVOICE_DATE { get; set; }
        public DateTime? CANCELLATION_DATE { get; set; }
        public string ISPAID { get; set; }


    }
}
