using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PayrollApp.Data;
using PayrollApp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PayrollApp.Controllers
{
    public class BenefitsInKindController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BenefitsInKindController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BenefitsInKind
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.BenefitsInKind.Include(b => b.Employee);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: BenefitsInKind/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var benefitInKind = await _context.BenefitsInKind
                .Include(b => b.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (benefitInKind == null)
            {
                return NotFound();
            }

            return View(benefitInKind);
        }

        // GET: BenefitsInKind/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FirstName");
            return View();
        }

        // POST: BenefitsInKind/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EmployeeId,Name,Amount")] BenefitInKind benefitInKind)
        {
            if (ModelState.IsValid)
            {
                _context.Add(benefitInKind);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FirstName", benefitInKind.EmployeeId);
            return View(benefitInKind);
        }

        // GET: BenefitsInKind/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var benefitInKind = await _context.BenefitsInKind.FindAsync(id);
            if (benefitInKind == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FirstName", benefitInKind.EmployeeId);
            return View(benefitInKind);
        }

        // POST: BenefitsInKind/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmployeeId,Name,Amount")] BenefitInKind benefitInKind)
        {
            if (id != benefitInKind.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(benefitInKind);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BenefitInKindExists(benefitInKind.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FirstName", benefitInKind.EmployeeId);
            return View(benefitInKind);
        }

        // GET: BenefitsInKind/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var benefitInKind = await _context.BenefitsInKind
                .Include(b => b.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (benefitInKind == null)
            {
                return NotFound();
            }

            return View(benefitInKind);
        }

        // POST: BenefitsInKind/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var benefitInKind = await _context.BenefitsInKind.FindAsync(id);
            _context.BenefitsInKind.Remove(benefitInKind);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BenefitInKindExists(int id)
        {
            return _context.BenefitsInKind.Any(e => e.Id == id);
        }
    }
}
