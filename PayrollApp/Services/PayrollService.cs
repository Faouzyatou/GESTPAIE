using PayrollApp.Data;
using PayrollApp.Models;
using System;
using System.Linq;

namespace PayrollApp.Services
{
    public class PayrollService
    {
        private readonly ApplicationDbContext _context;

        public PayrollService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Payslip CalculatePayslip(int employeeId, DateTime date)
        {
            var employee = _context.Employees.Find(employeeId);
            if (employee == null)
            {
                return null;
            }

            var payrollItems = _context.PayrollItems.ToList();
            var salaryGrid = _context.SalaryGrids.FirstOrDefault(g => g.Category == employee.Category && g.Echelon == employee.Echelon);
            var benefitsInKind = _context.BenefitsInKind.Where(b => b.EmployeeId == employeeId).ToList();

            var payslip = new Payslip
            {
                EmployeeId = employeeId,
                Date = date,
                GrossSalary = 0,
                NetSalary = 0,
                PayslipItems = new System.Collections.Generic.List<PayslipItem>()
            };

            // Calculate Seniority
            var seniorityInYears = (date.Year - employee.HireDate.Year - 1) +
                                 (((date.Month > employee.HireDate.Month) ||
                                 ((date.Month == employee.HireDate.Month) && (date.Day >= employee.HireDate.Day))) ? 1 : 0);

            var seniorityPercentage = 0;
            if (seniorityInYears >= 2)
            {
                seniorityPercentage = seniorityInYears * 2;
            }

            // Get Base Salary from Salary Grid
            var baseSalary = salaryGrid?.Amount ?? 0;

            // Calculate Seniority Amount
            var seniorityAmount = baseSalary * seniorityPercentage / 100;

            // Calculate Taxable Gains
            var taxableGains = payrollItems
                .Where(p => p.Category == PayrollItemCategory.Gain)
                .Sum(p => GetPayrollItemAmount(p, baseSalary, seniorityAmount));

            var totalTaxableGross = baseSalary + seniorityAmount + taxableGains;

            // Calculate Benefits in Kind
            var totalBenefitsInKind = 0m;
            foreach (var benefit in benefitsInKind)
            {
                var benefitAmount = 0m;
                switch (benefit.Name.ToLower())
                {
                    case "logement":
                        benefitAmount = Math.Min(benefit.Amount, totalTaxableGross * 0.15m);
                        break;
                    case "vehicule":
                        benefitAmount = Math.Min(benefit.Amount, totalTaxableGross * 0.10m);
                        break;
                    case "eau":
                        benefitAmount = Math.Min(benefit.Amount, totalTaxableGross * 0.02m);
                        break;
                    case "electricite":
                        benefitAmount = Math.Min(benefit.Amount, totalTaxableGross * 0.04m);
                        break;
                    case "domesticite":
                        benefitAmount = Math.Min(benefit.Amount, totalTaxableGross * 0.05m);
                        break;
                }
                totalBenefitsInKind += benefitAmount;
            }

            var taxableSalary = totalTaxableGross + totalBenefitsInKind;

            // Calculate CNPS
            var cnps = taxableSalary * 0.042m;

            // Calculate IRPP
            var irppBase = taxableSalary * 0.7m - cnps - 41667;
            var irpp = 0m;
            if (irppBase <= 166666)
            {
                irpp = irppBase * 0.1m;
            }
            else if (irppBase <= 250000)
            {
                irpp = (irppBase - 166667) * 0.15m + 16667;
            }
            else if (irppBase <= 416666)
            {
                irpp = (irppBase - 250001) * 0.25m + 33333;
            }
            else
            {
                irpp = (irppBase - 416667) * 0.35m + 75000;
            }

            // Calculate CAC on IRPP
            var cac = irpp * 0.01m;

            // Calculate TDL
            var tdl = 0m;
            if (baseSalary > 62000 && baseSalary <= 75000) tdl = 250;
            else if (baseSalary > 75000 && baseSalary <= 100000) tdl = 500;
            else if (baseSalary > 100000 && baseSalary <= 125000) tdl = 750;
            else if (baseSalary > 125000 && baseSalary <= 150000) tdl = 1000;
            else if (baseSalary > 150000 && baseSalary <= 200000) tdl = 1250;
            else if (baseSalary > 200000 && baseSalary <= 250000) tdl = 1500;
            else if (baseSalary > 250000 && baseSalary <= 300000) tdl = 2000;
            else if (baseSalary > 300000 && baseSalary <= 500000) tdl = 2250;
            else if (baseSalary > 500000) tdl = 2500;

            // Calculate RAV
            var rav = 0m;
            if (taxableSalary > 50000 && taxableSalary <= 100000) rav = 750;
            else if (taxableSalary > 100000 && taxableSalary <= 200000) rav = 1950;
            else if (taxableSalary > 200000 && taxableSalary <= 300000) rav = 3250;
            else if (taxableSalary > 300000 && taxableSalary <= 400000) rav = 4550;
            else if (taxableSalary > 400000 && taxableSalary <= 500000) rav = 5850;
            else if (taxableSalary > 500000 && taxableSalary <= 600000) rav = 7150;
            else if (taxableSalary > 600000 && taxableSalary <= 700000) rav = 8450;
            else if (taxableSalary > 700000 && taxableSalary <= 800000) rav = 9750;
            else if (taxableSalary > 800000 && taxableSalary <= 900000) rav = 11050;
            else if (taxableSalary > 900000 && taxableSalary <= 1000000) rav = 12350;
            else if (taxableSalary > 1000000) rav = 13000;

            // Calculate Credit Foncier
            var creditFoncier = taxableSalary * 0.01m;

            // Calculate Total Deductions
            var totalDeductions = cnps + irpp + cac + tdl + rav + creditFoncier;

            // Calculate Net Salary
            var netSalary = totalTaxableGross - totalDeductions;

            // Populate Payslip
            payslip.GrossSalary = totalTaxableGross;
            payslip.NetSalary = netSalary;
            payslip.PayslipItems.Add(new PayslipItem { Code = "BASE", Label = "Salaire de base", Amount = baseSalary });
            payslip.PayslipItems.Add(new PayslipItem { Code = "ANCIEN", Label = "Ancienneté", Amount = seniorityAmount });
            payslip.PayslipItems.Add(new PayslipItem { Code = "AVNAT", Label = "Avantages en nature", Amount = totalBenefitsInKind });
            payslip.PayslipItems.Add(new PayslipItem { Code = "CNPS", Label = "CNPS", Amount = -cnps });
            payslip.PayslipItems.Add(new PayslipItem { Code = "IRPP", Label = "IRPP", Amount = -irpp });
            payslip.PayslipItems.Add(new PayslipItem { Code = "CAC", Label = "CAC IRPP", Amount = -cac });
            payslip.PayslipItems.Add(new PayslipItem { Code = "TDL", Label = "TDL", Amount = -tdl });
            payslip.PayslipItems.Add(new PayslipItem { Code = "RAV", Label = "RAV", Amount = -rav });
            payslip.PayslipItems.Add(new PayslipItem { Code = "CRFON", Label = "Crédit Foncier", Amount = -creditFoncier });

            // Save payslip to history
            var payslipHistory = new PayslipHistory
            {
                EmployeeId = employeeId,
                Date = date,
                GrossSalary = payslip.GrossSalary,
                NetSalary = payslip.NetSalary,
                PayslipItemsJson = System.Text.Json.JsonSerializer.Serialize(payslip.PayslipItems)
            };
            _context.PayslipHistories.Add(payslipHistory);
            _context.SaveChanges();

            // Calculate IRPP Regularization in December
            if (date.Month == 12)
            {
                var annualTaxableSalary = _context.PayslipHistories
                    .Where(p => p.EmployeeId == employeeId && p.Date.Year == date.Year)
                    .Sum(p => p.GrossSalary + System.Text.Json.JsonSerializer.Deserialize<System.Collections.Generic.List<PayslipItem>>(p.PayslipItemsJson).FirstOrDefault(i => i.Code == "AVNAT")?.Amount ?? 0);

                var annualIrpp = CalculateAnnualIRPP(annualTaxableSalary);
                var monthlyIrppSum = _context.PayslipHistories
                    .Where(p => p.EmployeeId == employeeId && p.Date.Year == date.Year)
                    .ToList()
                    .Sum(p =>
                    {
                        var items = System.Text.Json.JsonSerializer.Deserialize<System.Collections.Generic.List<PayslipItem>>(p.PayslipItemsJson);
                        return items.FirstOrDefault(i => i.Code == "IRPP")?.Amount ?? 0;
                    });

                var irppRegularization = annualIrpp + monthlyIrppSum; // monthlyIrppSum is negative

                payslip.PayslipItems.Add(new PayslipItem { Code = "REGUL_IRPP", Label = "Régularisation IRPP", Amount = irppRegularization });
                payslip.NetSalary += irppRegularization;
            }

            return payslip;
        }

        private decimal CalculateAnnualIRPP(decimal annualTaxableSalary)
        {
            var annualIrpp = 0m;
            if (annualTaxableSalary <= 2000000)
            {
                annualIrpp = annualTaxableSalary * 0.1m;
            }
            else if (annualTaxableSalary <= 3000000)
            {
                annualIrpp = (annualTaxableSalary - 2000000) * 0.15m + 200000;
            }
            else if (annualTaxableSalary <= 5000000)
            {
                annualIrpp = (annualTaxableSalary - 3000000) * 0.25m + 350000;
            }
            else
            {
                annualIrpp = (annualTaxableSalary - 5000000) * 0.35m + 850000;
            }
            return annualIrpp;
        }

        private decimal GetPayrollItemAmount(PayrollItem item, decimal baseSalary, decimal seniorityAmount)
        {
            // This is a placeholder for the actual payroll item amount calculation logic.
            // I will implement the detailed calculation logic in the next steps.
            return 0;
        }
    }

    public class Payslip
    {
        public int EmployeeId { get; set; }
        public DateTime Date { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal NetSalary { get; set; }
        public System.Collections.Generic.List<PayslipItem> PayslipItems { get; set; }
    }

    public class PayslipItem
    {
        public string Code { get; set; }
        public string Label { get; set; }
        public decimal Amount { get; set; }
    }
}
