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
    public class SalariesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalariesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Salaries
        public async Task<IActionResult> Index()
        {
            var salaries = await _context.Salaries
                .Include(s => s.Employee)
                .ToListAsync();
            return View(salaries);
        }

        // GET: Salaries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salary = await _context.Salaries
                .Include(s => s.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (salary == null)
            {
                return NotFound();
            }

            return View(salary);
        }

        // POST: Salaries/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var salary = await _context.Salaries.FindAsync(id);
            if (salary == null)
            {
                return NotFound();
            }

            salary.Status = SalaryStatus.Approved;
            salary.ApprovedBy = User?.Identity?.Name ?? "System";
            salary.ApprovedDate = DateTime.Now;
            salary.RejectionReason = null;
            salary.ModifiedOn = DateTime.Now;

            _context.Update(salary);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Salaries/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string rejectionReason)
        {
            var salary = await _context.Salaries.FindAsync(id);
            if (salary == null)
            {
                return NotFound();
            }

            salary.Status = SalaryStatus.Rejected;
            salary.ApprovedBy = User?.Identity?.Name ?? "System";
            salary.ApprovedDate = DateTime.Now;
            salary.RejectionReason = string.IsNullOrWhiteSpace(rejectionReason) ? "Rejected" : rejectionReason;
            salary.ModifiedOn = DateTime.Now;

            _context.Update(salary);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Salaries/BulkUpdate
        public IActionResult BulkUpdate()
        {
            ViewBag.EmployeeOptions = new MultiSelectList(
                _context.Employees.OrderBy(e => e.FirstName).ThenBy(e => e.LastName),
                "Id",
                "FullName");

            var model = new BulkSalaryUpdateViewModel
            {
                EffectiveDate = DateTime.Now
            };

            return View(model);
        }

        // POST: Salaries/BulkUpdate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkUpdate(BulkSalaryUpdateViewModel model)
        {
            if (model.EmployeeIds == null || model.EmployeeIds.Count == 0)
            {
                ModelState.AddModelError("EmployeeIds", "Please select at least one employee.");
            }

            var percent = model.PercentageIncrease ?? 0m;
            var flat = model.FlatIncrease ?? 0m;
            if (percent == 0m && flat == 0m)
            {
                ModelState.AddModelError(string.Empty, "Provide a percentage or a flat increase.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.EmployeeOptions = new MultiSelectList(
                    _context.Employees.OrderBy(e => e.FirstName).ThenBy(e => e.LastName),
                    "Id",
                    "FullName",
                    model.EmployeeIds);

                return View(model);
            }

            var updatedCount = 0;
            var skippedCount = 0;
            var reason = string.IsNullOrWhiteSpace(model.Reason) ? "Bulk Salary Update" : model.Reason;

            foreach (var employeeId in model.EmployeeIds)
            {
                var salary = await _context.Salaries
                    .Where(s => s.EmployeeId == employeeId)
                    .OrderByDescending(s => s.EffectiveDate)
                    .FirstOrDefaultAsync();

                if (salary == null)
                {
                    skippedCount++;
                    continue;
                }

                var oldTotal = salary.TotalSalary;
                var newBasic = salary.BasicSalary;

                if (percent != 0m)
                {
                    newBasic += salary.BasicSalary * (percent / 100m);
                }

                if (flat != 0m)
                {
                    newBasic += flat;
                }

                if (newBasic < 0m)
                {
                    newBasic = 0m;
                }

                salary.BasicSalary = newBasic;
                salary.EffectiveDate = model.EffectiveDate;
                salary.Status = SalaryStatus.Pending;
                salary.ApprovedBy = null;
                salary.ApprovedDate = null;
                salary.RejectionReason = null;
                salary.ModifiedOn = DateTime.Now;

                var newTotal = salary.TotalSalary;
                if (newTotal != oldTotal)
                {
                    var salaryHistory = new SalaryHistory
                    {
                        EmployeeId = salary.EmployeeId,
                        OldSalary = oldTotal,
                        NewSalary = newTotal,
                        EffectiveDate = model.EffectiveDate,
                        Reason = reason,
                        Notes = "Bulk update",
                        CreatedOn = DateTime.Now
                    };
                    _context.SalaryHistories.Add(salaryHistory);
                    updatedCount++;
                }
                else
                {
                    skippedCount++;
                }
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Bulk update complete. Updated: {updatedCount}, Skipped: {skippedCount}.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Salaries/Reports
        public async Task<IActionResult> Reports()
        {
            var salaries = await _context.Salaries
                .Include(s => s.Employee)
                .ToListAsync();

            var report = new CompensationReportViewModel
            {
                TotalEmployees = await _context.Employees.CountAsync(),
                EmployeesWithSalary = salaries.Count,
                TotalPayroll = salaries.Sum(s => s.TotalSalary),
                AverageSalary = salaries.Count > 0 ? salaries.Average(s => s.TotalSalary) : 0m
            };

            report.DepartmentSummaries = salaries
                .Where(s => s.Employee != null)
                .GroupBy(s => string.IsNullOrWhiteSpace(s.Employee.Department) ? "Unassigned" : s.Employee.Department)
                .Select(g => new DepartmentCompSummary
                {
                    Department = g.Key,
                    EmployeeCount = g.Count(),
                    TotalPayroll = g.Sum(s => s.TotalSalary),
                    AverageSalary = g.Count() > 0 ? g.Average(s => s.TotalSalary) : 0m
                })
                .OrderByDescending(d => d.TotalPayroll)
                .ToList();

            report.StatusSummaries = salaries
                .GroupBy(s => s.Status)
                .Select(g => new SalaryStatusSummary
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(s => s.Count)
                .ToList();

            return View(report);
        }

        // GET: Salaries/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
            return View();
        }

        // POST: Salaries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Salary salary)
        {
            salary.CreatedOn = DateTime.Now;
            salary.Status = SalaryStatus.Pending;
            salary.ApprovedBy = null;
            salary.ApprovedDate = null;
            salary.RejectionReason = null;
            if (ModelState.IsValid)
            {
                // Check if there's an existing salary for this employee
                var existingSalary = await _context.Salaries
                    .Where(s => s.EmployeeId == salary.EmployeeId)
                    .OrderByDescending(s => s.EffectiveDate)
                    .FirstOrDefaultAsync();

                if (existingSalary != null)
                {
                    // Create salary history record
                    var salaryHistory = new SalaryHistory
                    {
                        EmployeeId = salary.EmployeeId,
                        OldSalary = existingSalary.TotalSalary,
                        NewSalary = salary.TotalSalary,
                        EffectiveDate = salary.EffectiveDate,
                        Reason = "Salary Update",
                        CreatedOn = DateTime.Now
                    };
                    _context.Add(salaryHistory);
                }

                _context.Add(salary);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", salary.EmployeeId);
            return View(salary);
        }

        // GET: Salaries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salary = await _context.Salaries.FindAsync(id);
            if (salary == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", salary.EmployeeId);
            return View(salary);
        }

        // POST: Salaries/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Salary salary)
        {
            if (id != salary.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingSalary = await _context.Salaries.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
                    
                    if (existingSalary != null && existingSalary.TotalSalary != salary.TotalSalary)
                    {
                        // Create salary history record
                        var salaryHistory = new SalaryHistory
                        {
                            EmployeeId = salary.EmployeeId,
                            OldSalary = existingSalary.TotalSalary,
                            NewSalary = salary.TotalSalary,
                            EffectiveDate = DateTime.Now,
                            Reason = "Salary Modification",
                            CreatedOn = DateTime.Now
                        };
                        _context.Add(salaryHistory);
                        salary.Status = SalaryStatus.Pending;
                        salary.ApprovedBy = null;
                        salary.ApprovedDate = null;
                        salary.RejectionReason = null;
                    }

                    salary.ModifiedOn = DateTime.Now;
                    _context.Update(salary);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalaryExists(salary.Id))
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
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", salary.EmployeeId);
            return View(salary);
        }

        // GET: Salaries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salary = await _context.Salaries
                .Include(s => s.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (salary == null)
            {
                return NotFound();
            }

            return View(salary);
        }

        // POST: Salaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var salary = await _context.Salaries.FindAsync(id);
            if (salary != null)
            {
                _context.Salaries.Remove(salary);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool SalaryExists(int id)
        {
            return _context.Salaries.Any(e => e.Id == id);
        }
    }
}
