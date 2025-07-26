using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayrollApp.Data;
using PayrollApp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PayrollApp.Controllers
{
    public class SalaryGridController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalaryGridController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SalaryGrid
        public async Task<IActionResult> Index()
        {
            return View(await _context.SalaryGrids.ToListAsync());
        }

        // GET: SalaryGrid/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salaryGrid = await _context.SalaryGrids
                .FirstOrDefaultAsync(m => m.Id == id);
            if (salaryGrid == null)
            {
                return NotFound();
            }

            return View(salaryGrid);
        }

        // GET: SalaryGrid/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SalaryGrid/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Category,Echelon,Amount")] SalaryGrid salaryGrid)
        {
            if (ModelState.IsValid)
            {
                _context.Add(salaryGrid);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(salaryGrid);
        }

        // GET: SalaryGrid/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salaryGrid = await _context.SalaryGrids.FindAsync(id);
            if (salaryGrid == null)
            {
                return NotFound();
            }
            return View(salaryGrid);
        }

        // POST: SalaryGrid/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Category,Echelon,Amount")] SalaryGrid salaryGrid)
        {
            if (id != salaryGrid.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(salaryGrid);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalaryGridExists(salaryGrid.Id))
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
            return View(salaryGrid);
        }

        // GET: SalaryGrid/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salaryGrid = await _context.SalaryGrids
                .FirstOrDefaultAsync(m => m.Id == id);
            if (salaryGrid == null)
            {
                return NotFound();
            }

            return View(salaryGrid);
        }

        // POST: SalaryGrid/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var salaryGrid = await _context.SalaryGrids.FindAsync(id);
            _context.SalaryGrids.Remove(salaryGrid);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalaryGridExists(int id)
        {
            return _context.SalaryGrids.Any(e => e.Id == id);
        }
    }
}
