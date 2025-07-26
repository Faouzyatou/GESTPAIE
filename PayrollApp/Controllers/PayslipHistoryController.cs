using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayrollApp.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PayrollApp.Controllers
{
    public class PayslipHistoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PayslipHistoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PayslipHistory
        public async Task<IActionResult> Index()
        {
            var payslipHistories = await _context.PayslipHistories.Include(p => p.Employee).ToListAsync();
            return View(payslipHistories);
        }

        // GET: PayslipHistory/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payslipHistory = await _context.PayslipHistories
                .Include(p => p.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (payslipHistory == null)
            {
                return NotFound();
            }

            return View(payslipHistory);
        }
    }
}
