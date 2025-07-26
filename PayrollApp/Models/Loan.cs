using System;

namespace PayrollApp.Models
{
    public class Loan
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public decimal Capital { get; set; }
        public decimal MonthlyPayment { get; set; }
        public int NumberOfMonths { get; set; }
        public DateTime StartDate { get; set; }
    }
}
