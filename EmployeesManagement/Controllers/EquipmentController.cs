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
    public class EquipmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EquipmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Equipment
        public async Task<IActionResult> Index()
        {
            var equipment = await _context.Equipment
                .Include(e => e.AssignedToEmployee)
                .ToListAsync();
            return View(equipment);
        }

        // GET: Equipment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipment = await _context.Equipment
                .Include(e => e.AssignedToEmployee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (equipment == null)
            {
                return NotFound();
            }

            return View(equipment);
        }

        // GET: Equipment/Assign/5
        public async Task<IActionResult> Assign(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipment = await _context.Equipment.FindAsync(id);
            if (equipment == null)
            {
                return NotFound();
            }

            if (equipment.Status == EquipmentStatus.Retired)
            {
                TempData["ErrorMessage"] = "Retired equipment cannot be assigned.";
                return RedirectToAction(nameof(Details), new { id });
            }

            ViewBag.EmployeeOptions = new SelectList(_context.Employees, "Id", "FullName");
            var model = new EquipmentAssignViewModel
            {
                EquipmentId = equipment.Id,
                EquipmentName = equipment.Name
            };
            return View(model);
        }

        // POST: Equipment/Assign/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(EquipmentAssignViewModel model)
        {
            var equipment = await _context.Equipment.FindAsync(model.EquipmentId);
            if (equipment == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.EmployeeOptions = new SelectList(_context.Employees, "Id", "FullName", model.EmployeeId);
                model.EquipmentName = equipment.Name;
                return View(model);
            }

            if (equipment.Status == EquipmentStatus.Retired)
            {
                TempData["ErrorMessage"] = "Retired equipment cannot be assigned.";
                return RedirectToAction(nameof(Details), new { id = equipment.Id });
            }

            equipment.AssignedToEmployeeId = model.EmployeeId;
            equipment.AssignmentDate = DateTime.Now;
            equipment.ReturnDate = null;
            equipment.Status = EquipmentStatus.Assigned;
            equipment.ModifiedOn = DateTime.Now;

            _context.Update(equipment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Equipment assigned successfully.";
            return RedirectToAction(nameof(Details), new { id = equipment.Id });
        }

        // POST: Equipment/Return/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(int id)
        {
            var equipment = await _context.Equipment.FindAsync(id);
            if (equipment == null)
            {
                return NotFound();
            }

            equipment.AssignedToEmployeeId = null;
            equipment.ReturnDate = DateTime.Now;
            equipment.Status = EquipmentStatus.Available;
            equipment.ModifiedOn = DateTime.Now;

            _context.Update(equipment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Equipment returned and marked as available.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Equipment/StartMaintenance/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartMaintenance(int id)
        {
            var equipment = await _context.Equipment.FindAsync(id);
            if (equipment == null)
            {
                return NotFound();
            }

            equipment.Status = EquipmentStatus.UnderMaintenance;
            equipment.ModifiedOn = DateTime.Now;
            _context.Update(equipment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Equipment marked as under maintenance.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Equipment/CompleteMaintenance/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteMaintenance(int id)
        {
            var equipment = await _context.Equipment.FindAsync(id);
            if (equipment == null)
            {
                return NotFound();
            }

            equipment.Status = equipment.AssignedToEmployeeId.HasValue
                ? EquipmentStatus.Assigned
                : EquipmentStatus.Available;
            equipment.ModifiedOn = DateTime.Now;
            _context.Update(equipment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Maintenance completed.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Equipment/Retire/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Retire(int id)
        {
            var equipment = await _context.Equipment.FindAsync(id);
            if (equipment == null)
            {
                return NotFound();
            }

            equipment.Status = EquipmentStatus.Retired;
            equipment.AssignedToEmployeeId = null;
            equipment.ReturnDate = DateTime.Now;
            equipment.ModifiedOn = DateTime.Now;
            _context.Update(equipment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Equipment retired.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Equipment/Create
        public IActionResult Create()
        {
            ViewData["AssignedToEmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
            return View();
        }

        // POST: Equipment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Equipment equipment)
        {
            equipment.CreatedOn = DateTime.Now;
            if (equipment.AssignedToEmployeeId.HasValue)
            {
                equipment.AssignmentDate = DateTime.Now;
                equipment.Status = EquipmentStatus.Assigned;
            }
            if (ModelState.IsValid)
            {
                _context.Add(equipment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AssignedToEmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", equipment.AssignedToEmployeeId);
            return View(equipment);
        }

        // GET: Equipment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipment = await _context.Equipment.FindAsync(id);
            if (equipment == null)
            {
                return NotFound();
            }
            ViewData["AssignedToEmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", equipment.AssignedToEmployeeId);
            return View(equipment);
        }

        // POST: Equipment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Equipment equipment)
        {
            if (id != equipment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    equipment.ModifiedOn = DateTime.Now;
                    _context.Update(equipment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EquipmentExists(equipment.Id))
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
            ViewData["AssignedToEmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", equipment.AssignedToEmployeeId);
            return View(equipment);
        }

        // GET: Equipment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipment = await _context.Equipment
                .Include(e => e.AssignedToEmployee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (equipment == null)
            {
                return NotFound();
            }

            return View(equipment);
        }

        // POST: Equipment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var equipment = await _context.Equipment.FindAsync(id);
            if (equipment != null)
            {
                _context.Equipment.Remove(equipment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Equipment/Audit
        public async Task<IActionResult> Audit()
        {
            var equipment = await _context.Equipment
                .Include(e => e.AssignedToEmployee)
                .ToListAsync();

            var report = new EquipmentAuditViewModel
            {
                TotalEquipment = equipment.Count,
                AssignedEquipment = equipment.Count(e => e.Status == EquipmentStatus.Assigned),
                AvailableEquipment = equipment.Count(e => e.Status == EquipmentStatus.Available),
                UnderMaintenanceEquipment = equipment.Count(e => e.Status == EquipmentStatus.UnderMaintenance),
                RetiredEquipment = equipment.Count(e => e.Status == EquipmentStatus.Retired),
                ByType = equipment.GroupBy(e => e.Type.ToString())
                    .Select(g => new EquipmentTypeCount
                    { 
                        Type = g.Key, 
                        Count = g.Count(), 
                        Value = g.Sum(e => e.PurchasePrice ?? 0) 
                    })
                    .OrderByDescending(x => x.Count)
                    .ToList(),
                AssignmentsSummary = equipment
                    .Where(e => e.AssignedToEmployee != null)
                    .GroupBy(e => new { e.AssignedToEmployee.FirstName, e.AssignedToEmployee.LastName })
                    .Select(g => new EmployeeAssignment 
                    { 
                        Employee = $"{g.Key.FirstName} {g.Key.LastName}", 
                        Count = g.Count() 
                    })
                    .OrderByDescending(x => x.Count)
                    .ToList()
            };

            return View(report);
        }

        private bool EquipmentExists(int id)
        {
            return _context.Equipment.Any(e => e.Id == id);
        }
    }
}
