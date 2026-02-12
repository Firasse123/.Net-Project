using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeesManagement.Data;
using EmployeesManagement.Models;

namespace EmployeesManagement.Controllers
{
    public class JobOpeningsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JobOpeningsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: JobOpenings
        public async Task<IActionResult> Index()
        {
            return View(await _context.JobOpenings.ToListAsync());
        }

        // GET: JobOpenings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobOpening = await _context.JobOpenings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobOpening == null)
            {
                return NotFound();
            }

            // Get candidates for this job opening
            var candidates = await _context.Candidates
                .Where(c => c.JobOpeningId == id)
                .OrderByDescending(c => c.ApplicationDate)
                .ToListAsync();

            ViewBag.Candidates = candidates;
            ViewBag.TotalCandidates = candidates.Count;
            ViewBag.PendingCandidates = candidates.Count(c => c.Status == CandidateStatus.Applied || c.Status == CandidateStatus.UnderReview);
            ViewBag.InterviewedCandidates = candidates.Count(c => c.Status == CandidateStatus.Interviewed);
            ViewBag.HiredCandidates = candidates.Count(c => c.Status == CandidateStatus.Hired);

            return View(jobOpening);
        }

        // POST: JobOpenings/ClosePosition/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClosePosition(int id)
        {
            var jobOpening = await _context.JobOpenings.FindAsync(id);
            if (jobOpening == null)
            {
                return NotFound();
            }

            jobOpening.Status = JobStatus.Closed;
            jobOpening.ClosingDate = DateTime.Now;
            jobOpening.ModifiedOn = DateTime.Now;
            _context.Update(jobOpening);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: JobOpenings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: JobOpenings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JobOpening jobOpening)
        {
            jobOpening.CreatedOn = DateTime.Now;
            jobOpening.PostedDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                _context.Add(jobOpening);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(jobOpening);
        }

        // GET: JobOpenings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobOpening = await _context.JobOpenings.FindAsync(id);
            if (jobOpening == null)
            {
                return NotFound();
            }
            return View(jobOpening);
        }

        // POST: JobOpenings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, JobOpening jobOpening)
        {
            if (id != jobOpening.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    jobOpening.ModifiedOn = DateTime.Now;
                    _context.Update(jobOpening);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobOpeningExists(jobOpening.Id))
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
            return View(jobOpening);
        }

        // GET: JobOpenings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobOpening = await _context.JobOpenings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobOpening == null)
            {
                return NotFound();
            }

            return View(jobOpening);
        }

        // POST: JobOpenings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jobOpening = await _context.JobOpenings.FindAsync(id);
            if (jobOpening != null)
            {
                _context.JobOpenings.Remove(jobOpening);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool JobOpeningExists(int id)
        {
            return _context.JobOpenings.Any(e => e.Id == id);
        }
    }
}
