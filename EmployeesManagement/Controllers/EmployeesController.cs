using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeesManagement.Data;
using EmployeesManagement.Models;

namespace EmployeesManagement.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Employees
        public async Task<IActionResult> Index(string status, string department, string searchString)
        {
            var employees = from e in _context.Employees select e;

            // Filter by status
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<EmployeeStatus>(status, out var statusEnum))
            {
                employees = employees.Where(e => e.Status == statusEnum);
            }

            // Filter by department
            if (!string.IsNullOrEmpty(department))
            {
                employees = employees.Where(e => e.Department == department);
            }

            // Search by name or email
            if (!string.IsNullOrEmpty(searchString))
            {
                employees = employees.Where(e => 
                    e.FirstName.Contains(searchString) || 
                    e.LastName.Contains(searchString) ||
                    e.EmailAddress.Contains(searchString) ||
                    e.EmpNo.Contains(searchString));
            }

            // Get unique departments for filter dropdown
            ViewBag.Departments = await _context.Employees
                .Where(e => !string.IsNullOrEmpty(e.Department))
                .Select(e => e.Department)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();

            ViewBag.CurrentStatus = status;
            ViewBag.CurrentDepartment = department;
            ViewBag.CurrentSearch = searchString;

            return View(await employees.ToListAsync());
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Manager)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (employee == null)
            {
                return NotFound();
            }

            // Get quick stats for this employee
            ViewBag.TotalBenefits = await _context.Benefits.CountAsync(b => b.EmployeeId == id && b.IsActive);
            ViewBag.AssignedEquipment = await _context.Equipment.CountAsync(e => e.AssignedToEmployeeId == id && e.Status == EquipmentStatus.Assigned);
            ViewBag.HasSalary = await _context.Salaries.AnyAsync(s => s.EmployeeId == id);

            return View(employee);
        }

        // GET: Employees/Create
        public async Task<IActionResult> Create()
        {
            // Get list of managers for dropdown
            ViewBag.Managers = await _context.Employees
                .Where(e => e.Status == EmployeeStatus.Active)
                .Select(e => new { e.Id, e.FullName })
                .ToListAsync();
            
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            employee.CreatedById = "wasiq";
            employee.CreatedOn = DateTime.Now;
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            // Get list of managers for dropdown (exclude current employee)
            ViewBag.Managers = await _context.Employees
                .Where(e => e.Status == EmployeeStatus.Active && e.Id != id)
                .Select(e => new { e.Id, e.FullName })
                .ToListAsync();
            
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    employee.ModifiedById = "wasiq";
                    employee.ModifiedOn = DateTime.Now;
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
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
            return View(employee);
        }

        // GET: Employees/Profile/5
        public async Task<IActionResult> Profile(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Manager)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            // Get related data
            var salary = await _context.Salaries
                .Where(s => s.EmployeeId == id)
                .FirstOrDefaultAsync();

            var salaryHistory = await _context.SalaryHistories
                .Where(sh => sh.EmployeeId == id)
                .OrderByDescending(sh => sh.EffectiveDate)
                .ToListAsync();

            var benefits = await _context.Benefits
                .Where(b => b.EmployeeId == id)
                .OrderByDescending(b => b.StartDate)
                .ToListAsync();

            var equipment = await _context.Equipment
                .Where(e => e.AssignedToEmployeeId == id)
                .OrderByDescending(e => e.AssignmentDate)
                .ToListAsync();

            // Pass data via ViewBag
            ViewBag.CurrentSalary = salary;
            ViewBag.SalaryHistory = salaryHistory;
            ViewBag.Benefits = benefits;
            ViewBag.Equipment = equipment;

            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}
