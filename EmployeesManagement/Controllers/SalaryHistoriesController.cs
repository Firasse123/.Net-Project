using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeesManagement.Data;

namespace EmployeesManagement.Controllers
{
    public class SalaryHistoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalaryHistoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SalaryHistories
        public async Task<IActionResult> Index()
        {
            var salaryHistories = await _context.SalaryHistories
                .Include(s => s.Employee)
                .OrderByDescending(s => s.EffectiveDate)
                .ToListAsync();
            return View(salaryHistories);
        }

        // GET: SalaryHistories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salaryHistory = await _context.SalaryHistories
                .Include(s => s.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (salaryHistory == null)
            {
                return NotFound();
            }

            return View(salaryHistory);
        }

        // GET: SalaryHistories/ByEmployee/5
        public async Task<IActionResult> ByEmployee(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salaryHistories = await _context.SalaryHistories
                .Include(s => s.Employee)
                .Where(s => s.EmployeeId == id)
                .OrderByDescending(s => s.EffectiveDate)
                .ToListAsync();

            if (!salaryHistories.Any())
            {
                return NotFound();
            }

            ViewData["EmployeeName"] = salaryHistories.First().Employee?.FullName;
            return View("Index", salaryHistories);
        }
    }
}
