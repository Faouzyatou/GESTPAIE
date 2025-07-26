namespace PayrollApp.Models
{
    public class PayrollItem
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Label { get; set; }
        public PayrollItemType Type { get; set; }
        public PayrollItemCategory Category { get; set; }
        public string Formula { get; set; }
        public bool IsProrated { get; set; }
        public bool AppearsOnPayslip { get; set; }
    }

    public enum PayrollItemType
    {
        Grid,
        Scale,
        Cumulative,
        CalculationBase,
        Formula,
        Condition
    }

    public enum PayrollItemCategory
    {
        Gain,
        SocialSecurityWithholding,
        FiscalWithholding,
        OtherWithholding,
        NonTaxableGain
    }
}
