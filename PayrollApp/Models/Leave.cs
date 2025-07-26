using System;

namespace PayrollApp.Models
{
    public class Leave
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsDeductible { get; set; }
    }
}
