using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayrollApp.Data;
using PayrollApp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PayrollApp.Controllers
{
    public class PayrollItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PayrollItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PayrollItems
        public async Task<IActionResult> Index()
        {
            return View(await _context.PayrollItems.ToListAsync());
        }

        // GET: PayrollItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payrollItem = await _context.PayrollItems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (payrollItem == null)
            {
                return NotFound();
            }

            return View(payrollItem);
        }

        // GET: PayrollItems/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PayrollItems/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Code,Label,Type,Category,Formula,IsProrated,AppearsOnPayslip")] PayrollItem payrollItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(payrollItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(payrollItem);
        }

        // GET: PayrollItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payrollItem = await _context.PayrollItems.FindAsync(id);
            if (payrollItem == null)
            {
                return NotFound();
            }
            return View(payrollItem);
        }

        // POST: PayrollItems/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code,Label,Type,Category,Formula,IsProrated,AppearsOnPayslip")] PayrollItem payrollItem)
        {
            if (id != payrollItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(payrollItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PayrollItemExists(payrollItem.Id))
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
            return View(payrollItem);
        }

        // GET: PayrollItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payrollItem = await _context.PayrollItems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (payrollItem == null)
            {
                return NotFound();
            }

            return View(payrollItem);
        }

        // POST: PayrollItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payrollItem = await _context.PayrollItems.FindAsync(id);
            _context.PayrollItems.Remove(payrollItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PayrollItemExists(int id)
        {
            return _context.PayrollItems.Any(e => e.Id == id);
        }
    }
}
