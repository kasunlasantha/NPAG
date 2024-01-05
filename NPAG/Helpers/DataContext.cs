using Microsoft.EntityFrameworkCore;
using NPAG.Entities;

namespace NPAG.Helpers
{
    public class DataContext : DbContext
    {

        //public DbSet<Users> Users { get; set; }
        public DbSet<NPAG_COMPANYDATA> NPAG_COMPANYDATAs { get; set; }

        public DbSet<CRM_PAYMENT> CRM_PAYMENTs { get; set; }

        public DbSet<CRM_INVOICES> CRM_INVOICESs { get; set; }
        
        public DbSet<NPAG_ERROR_PAYMENT> NPAG_ERROR_PAYMENTs { get; set; }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NPAG_COMPANYDATA>()
                .HasKey(c => new { c.BRANCHCODE, c.COMPCODE });

            modelBuilder.Entity<CRM_PAYMENT>()
                .HasKey(c => new { c.PAYMENTDATE, c.ORDER_NUMBER, c.AMOUNT });

            modelBuilder.Entity<CRM_INVOICES>()
               .HasKey(c => new { c.ORDER_NO, c.CRM_GETID});

            modelBuilder.Entity<NPAG_ERROR_PAYMENT>()
               .HasKey(c => new { c.PAYMENTID});
        }


        //     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //	//optionsBuilder.UseOracle(@"User Id=blog;Password=<password>;Data Source=pdborcl;");
        //             //optionsBuilder.UseOracle(@"User Id=hr;Password=hr;Data Source=localhost:1521/orcl;");
        //}
    }
}
