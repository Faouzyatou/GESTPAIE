using Microsoft.AspNetCore.Mvc;
using PayrollApp.Services;
using System;
using System.Threading.Tasks;

namespace PayrollApp.Controllers
{
    public class PayrollController : Controller
    {
        private readonly PayrollService _payrollService;

        public PayrollController(PayrollService payrollService)
        {
            _payrollService = payrollService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Calculate(int employeeId, DateTime date)
        {
            var payslip = _payrollService.CalculatePayslip(employeeId, date);
            if (payslip == null)
            {
                return NotFound();
            }
            return View("Payslip", payslip);
        }

        public IActionResult ExportToCsv(int employeeId, DateTime date)
        {
            var payslip = _payrollService.CalculatePayslip(employeeId, date);
            if (payslip == null)
            {
                return NotFound();
            }

            var builder = new System.Text.StringBuilder();
            builder.AppendLine("Code,Label,Amount");
            foreach (var item in payslip.PayslipItems)
            {
                builder.AppendLine($"{item.Code},{item.Label},{item.Amount}");
            }

            return File(System.Text.Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", $"payslip-{employeeId}-{date:yyyy-MM-dd}.csv");
        }
    }
}
