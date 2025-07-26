using Microsoft.EntityFrameworkCore;
using PayrollApp.Models;

namespace PayrollApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<PayrollItem> PayrollItems { get; set; }
        public DbSet<SalaryGrid> SalaryGrids { get; set; }
        public DbSet<BenefitInKind> BenefitsInKind { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<Leave> Leaves { get; set; }
        public DbSet<PayslipHistory> PayslipHistories { get; set; }
    }
}
