namespace PayrollApp.Models
{
    public class BenefitInKind
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
    }
}
