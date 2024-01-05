
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace NPAG.Entities
{
    [Table("NPAG_COMPANYDATA")]
    public class NPAG_COMPANYDATA
    {
        [Required]
        public decimal? COMPCODE { get; set; }

        [StringLength(50)]
        public string COMPNAME { get; set; }

        [Required]
        public decimal? BRANCHCODE { get; set; }

        [StringLength(50)]
        public string BRANCHNAME { get; set; }

        public decimal? SECUCODE { get; set; }

        [StringLength(20)]
        public string PAYMENTMODE { get; set; }

    }
}
