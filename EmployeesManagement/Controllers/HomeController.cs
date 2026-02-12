using System.Diagnostics;
using EmployeesManagement.Models;
using EmployeesManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeesManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Dashboard Statistics
            ViewBag.TotalEmployees = await _context.Employees.CountAsync(e => e.Status == EmployeeStatus.Active);
            ViewBag.TotalOpenPositions = await _context.JobOpenings.CountAsync(j => j.Status == JobStatus.Open);
            ViewBag.PendingCandidates = await _context.Candidates.CountAsync(c => c.Status == CandidateStatus.UnderReview || c.Status == CandidateStatus.Interviewed);
            ViewBag.TotalEquipment = await _context.Equipment.CountAsync();
            ViewBag.AssignedEquipment = await _context.Equipment.CountAsync(e => e.Status == EquipmentStatus.Assigned);
            
            // Calculate monthly payroll (sum all allowances directly in query)
            ViewBag.MonthlyPayroll = await _context.Salaries
                .SumAsync(s => s.BasicSalary + 
                              (s.HousingAllowance ?? 0) + 
                              (s.TransportAllowance ?? 0) + 
                              (s.MedicalAllowance ?? 0) + 
                              (s.OtherAllowances ?? 0));

            // Recent activities
            var recentHires = await _context.Employees
                .Where(e => e.HireDate.HasValue && e.HireDate.Value >= DateTime.Now.AddDays(-30))
                .OrderByDescending(e => e.HireDate)
                .Take(5)
                .ToListAsync();

            var recentCandidates = await _context.Candidates
                .Include(c => c.JobOpening)
                .OrderByDescending(c => c.ApplicationDate)
                .Take(5)
                .ToListAsync();

            ViewBag.RecentHires = recentHires;
            ViewBag.RecentCandidates = recentCandidates;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
