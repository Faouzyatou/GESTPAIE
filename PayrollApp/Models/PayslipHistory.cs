using System;
using System.Collections.Generic;

namespace PayrollApp.Models
{
    public class PayslipHistory
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public DateTime Date { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal NetSalary { get; set; }
        public string PayslipItemsJson { get; set; }
    }
}
