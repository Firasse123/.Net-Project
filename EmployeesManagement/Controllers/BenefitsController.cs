using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeesManagement.Data;
using EmployeesManagement.Models;

namespace EmployeesManagement.Controllers
{
    public class BenefitsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BenefitsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Benefits
        public async Task<IActionResult> Index()
        {
            var benefits = await _context.Benefits
                .Include(b => b.Employee)
                .ToListAsync();
            return View(benefits);
        }

        // GET: Benefits/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var benefit = await _context.Benefits
                .Include(b => b.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (benefit == null)
            {
                return NotFound();
            }

            return View(benefit);
        }

        // GET: Benefits/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
            return View();
        }

        // POST: Benefits/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Benefit benefit)
        {
            benefit.CreatedOn = DateTime.Now;
            if (ModelState.IsValid)
            {
                _context.Add(benefit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", benefit.EmployeeId);
            return View(benefit);
        }

        // GET: Benefits/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var benefit = await _context.Benefits.FindAsync(id);
            if (benefit == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", benefit.EmployeeId);
            return View(benefit);
        }

        // POST: Benefits/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Benefit benefit)
        {
            if (id != benefit.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    benefit.ModifiedOn = DateTime.Now;
                    _context.Update(benefit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BenefitExists(benefit.Id))
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
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", benefit.EmployeeId);
            return View(benefit);
        }

        // POST: Benefits/Activate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(int id)
        {
            var benefit = await _context.Benefits.FindAsync(id);
            if (benefit == null)
            {
                return NotFound();
            }

            benefit.IsActive = true;
            benefit.EndDate = null;
            benefit.ModifiedOn = DateTime.Now;
            _context.Update(benefit);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: Benefits/Deactivate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            var benefit = await _context.Benefits.FindAsync(id);
            if (benefit == null)
            {
                return NotFound();
            }

            benefit.IsActive = false;
            benefit.EndDate = DateTime.Now;
            benefit.ModifiedOn = DateTime.Now;
            _context.Update(benefit);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Benefits/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var benefit = await _context.Benefits
                .Include(b => b.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (benefit == null)
            {
                return NotFound();
            }

            return View(benefit);
        }

        // POST: Benefits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var benefit = await _context.Benefits.FindAsync(id);
            if (benefit != null)
            {
                _context.Benefits.Remove(benefit);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BenefitExists(int id)
        {
            return _context.Benefits.Any(e => e.Id == id);
        }
    }
}
