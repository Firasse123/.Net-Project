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
    public class CandidatesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CandidatesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Candidates
        public async Task<IActionResult> Index(CandidateStatus? status, int? jobOpeningId, string searchString)
        {
            var candidates = _context.Candidates
                .Include(c => c.JobOpening)
                .AsQueryable();

            // Filter by status
            if (status.HasValue)
            {
                candidates = candidates.Where(c => c.Status == status.Value);
            }

            // Filter by job opening
            if (jobOpeningId.HasValue)
            {
                candidates = candidates.Where(c => c.JobOpeningId == jobOpeningId.Value);
            }

            // Search by name or email
            if (!string.IsNullOrEmpty(searchString))
            {
                candidates = candidates.Where(c => 
                    c.FirstName.Contains(searchString) || 
                    c.LastName.Contains(searchString) || 
                    c.Email.Contains(searchString));
            }

            // Get statistics for pipeline visualization
            var totalCandidates = await _context.Candidates.CountAsync();
            var appliedCount = await _context.Candidates.CountAsync(c => c.Status == CandidateStatus.Applied);
            var underReviewCount = await _context.Candidates.CountAsync(c => c.Status == CandidateStatus.UnderReview);
            var interviewedCount = await _context.Candidates.CountAsync(c => c.Status == CandidateStatus.Interviewed);
            var offeredCount = await _context.Candidates.CountAsync(c => c.Status == CandidateStatus.Offered);
            var hiredCount = await _context.Candidates.CountAsync(c => c.Status == CandidateStatus.Hired);
            var rejectedCount = await _context.Candidates.CountAsync(c => c.Status == CandidateStatus.Rejected);

            ViewBag.AppliedCount = appliedCount;
            ViewBag.UnderReviewCount = underReviewCount;
            ViewBag.InterviewedCount = interviewedCount;
            ViewBag.OfferedCount = offeredCount;
            ViewBag.HiredCount = hiredCount;
            ViewBag.RejectedCount = rejectedCount;

            ViewBag.CurrentStatus = status;
            ViewBag.CurrentJobOpening = jobOpeningId;
            ViewBag.CurrentSearch = searchString;
            ViewBag.JobOpenings = new SelectList(await _context.JobOpenings.Where(j => j.Status == JobStatus.Open).ToListAsync(), "Id", "JobTitle");

            return View(await candidates.OrderByDescending(c => c.ApplicationDate).ToListAsync());
        }

        // GET: Candidates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var candidate = await _context.Candidates
                .Include(c => c.JobOpening)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (candidate == null)
            {
                return NotFound();
            }

            return View(candidate);
        }

        // GET: Candidates/Create
        public IActionResult Create()
        {
            ViewData["JobOpeningId"] = new SelectList(_context.JobOpenings, "Id", "JobTitle");
            return View();
        }

        // POST: Candidates/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Candidate candidate)
        {
            candidate.CreatedOn = DateTime.Now;
            candidate.ApplicationDate = DateTime.Now;
            var jobOpening = await _context.JobOpenings.FindAsync(candidate.JobOpeningId);
            if (jobOpening == null)
            {
                ModelState.AddModelError("JobOpeningId", "Selected job opening was not found.");
            }
            else if (jobOpening.Status != JobStatus.Open)
            {
                ModelState.AddModelError("JobOpeningId", "This job opening is not open for applications.");
            }
            if (ModelState.IsValid)
            {
                _context.Add(candidate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["JobOpeningId"] = new SelectList(_context.JobOpenings, "Id", "JobTitle", candidate.JobOpeningId);
            return View(candidate);
        }

        // GET: Candidates/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var candidate = await _context.Candidates.FindAsync(id);
            if (candidate == null)
            {
                return NotFound();
            }
            ViewData["JobOpeningId"] = new SelectList(_context.JobOpenings, "Id", "JobTitle", candidate.JobOpeningId);
            return View(candidate);
        }

        // POST: Candidates/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Candidate candidate)
        {
            if (id != candidate.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    candidate.ModifiedOn = DateTime.Now;
                    _context.Update(candidate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CandidateExists(candidate.Id))
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
            ViewData["JobOpeningId"] = new SelectList(_context.JobOpenings, "Id", "JobTitle", candidate.JobOpeningId);
            return View(candidate);
        }

        // POST: Candidates/HireCandidate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HireCandidate(int id)
        {
            var candidate = await _context.Candidates
                .Include(c => c.JobOpening)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (candidate == null)
            {
                return NotFound();
            }

            // Check if candidate is already hired
            if (candidate.Status == CandidateStatus.Hired)
            {
                TempData["ErrorMessage"] = "This candidate has already been hired.";
                return RedirectToAction(nameof(Details), new { id = candidate.Id });
            }

            if (candidate.Status != CandidateStatus.Offered)
            {
                TempData["ErrorMessage"] = "Candidate must be in Offered status before hiring.";
                return RedirectToAction(nameof(Details), new { id = candidate.Id });
            }

            if (candidate.JobOpening != null && candidate.JobOpening.Status != JobStatus.Open)
            {
                TempData["ErrorMessage"] = "Cannot hire for a closed or on-hold job opening.";
                return RedirectToAction(nameof(Details), new { id = candidate.Id });
            }

            // Create new Employee from Candidate
            var employee = new Employee
            {
                FirstName = candidate.FirstName,
                LastName = candidate.LastName,
                MiddleName = "",
                EmailAddress = candidate.Email,
                PhoneNumber = int.TryParse(candidate.Phone, out int phoneNum) ? phoneNum : 0,
                Designation = candidate.JobOpening?.JobTitle ?? "Not Specified",
                Department = candidate.JobOpening?.Department ?? "Not Specified",
                HireDate = DateTime.Now,
                Status = EmployeeStatus.Active,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
                EmpNo = $"EMP{DateTime.Now.Year}{(await _context.Employees.CountAsync() + 1):D4}",
                Address = "",
                Country = "",
                DateOfBirth = DateTime.Now.AddYears(-25)
            };

            try
            {
                // Add new employee
                _context.Employees.Add(employee);
                
                // Update candidate status
                candidate.Status = CandidateStatus.Hired;
                candidate.ModifiedOn = DateTime.Now;
                _context.Update(candidate);

                // Check if we should close the job opening (e.g., if all positions are filled)
                // For now, we'll leave it open - HR can manually close it when needed

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"{candidate.FirstName} {candidate.LastName} has been successfully hired!";
                return RedirectToAction("Details", "Employees", new { id = employee.Id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error hiring candidate: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id = candidate.Id });
            }
        }

        // POST: Candidates/ScheduleInterview/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ScheduleInterview(int id, DateTime interviewDate)
        {
            var candidate = await _context.Candidates.FindAsync(id);
            if (candidate == null)
            {
                return NotFound();
            }

            if (interviewDate < DateTime.Now)
            {
                TempData["ErrorMessage"] = "Interview date must be in the future.";
                return RedirectToAction(nameof(Details), new { id = candidate.Id });
            }

            if (candidate.Status != CandidateStatus.Applied && candidate.Status != CandidateStatus.UnderReview)
            {
                TempData["ErrorMessage"] = "Only Applied or Under Review candidates can be scheduled for interview.";
                return RedirectToAction(nameof(Details), new { id = candidate.Id });
            }

            candidate.Status = CandidateStatus.Interviewed;
            candidate.InterviewDate = interviewDate;
            candidate.ModifiedOn = DateTime.Now;
            _context.Update(candidate);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Interview scheduled successfully.";
            return RedirectToAction(nameof(Details), new { id = candidate.Id });
        }

        // POST: Candidates/MakeOffer/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeOffer(int id)
        {
            var candidate = await _context.Candidates.FindAsync(id);
            if (candidate == null)
            {
                return NotFound();
            }

            if (candidate.Status != CandidateStatus.Interviewed)
            {
                TempData["ErrorMessage"] = "Only Interviewed candidates can receive an offer.";
                return RedirectToAction(nameof(Details), new { id = candidate.Id });
            }

            candidate.Status = CandidateStatus.Offered;
            candidate.ModifiedOn = DateTime.Now;
            _context.Update(candidate);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Job offer sent to candidate.";
            return RedirectToAction(nameof(Details), new { id = candidate.Id });
        }

        // POST: Candidates/RejectCandidate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectCandidate(int id)
        {
            var candidate = await _context.Candidates.FindAsync(id);
            if (candidate == null)
            {
                return NotFound();
            }

            if (candidate.Status == CandidateStatus.Hired)
            {
                TempData["ErrorMessage"] = "Hired candidates cannot be rejected.";
                return RedirectToAction(nameof(Details), new { id = candidate.Id });
            }

            candidate.Status = CandidateStatus.Rejected;
            candidate.ModifiedOn = DateTime.Now;
            _context.Update(candidate);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Candidate has been rejected.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Candidates/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var candidate = await _context.Candidates
                .Include(c => c.JobOpening)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (candidate == null)
            {
                return NotFound();
            }

            return View(candidate);
        }

        // POST: Candidates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var candidate = await _context.Candidates.FindAsync(id);
            if (candidate != null)
            {
                _context.Candidates.Remove(candidate);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Candidates/Stats
        public async Task<IActionResult> Stats()
        {
            var candidates = await _context.Candidates
                .Include(c => c.JobOpening)
                .ToListAsync();

            var report = new RecruitmentStatsViewModel
            {
                TotalCandidates = candidates.Count,
                Applied = candidates.Count(c => c.Status == CandidateStatus.Applied),
                UnderReview = candidates.Count(c => c.Status == CandidateStatus.UnderReview),
                Interviewed = candidates.Count(c => c.Status == CandidateStatus.Interviewed),
                Offered = candidates.Count(c => c.Status == CandidateStatus.Offered),
                Hired = candidates.Count(c => c.Status == CandidateStatus.Hired),
                Rejected = candidates.Count(c => c.Status == CandidateStatus.Rejected),
                HireRate = candidates.Count > 0 ? (decimal)candidates.Count(c => c.Status == CandidateStatus.Hired) / candidates.Count * 100 : 0,
                ByJobOpening = candidates
                    .Where(c => c.JobOpening != null)
                    .GroupBy(c => c.JobOpening.JobTitle)
                    .Select(g => new JobPositionStats
                    { 
                        Position = g.Key, 
                        Total = g.Count(),
                        Applied = g.Count(x => x.Status == CandidateStatus.Applied),
                        Interviewed = g.Count(x => x.Status == CandidateStatus.Interviewed),
                        Hired = g.Count(x => x.Status == CandidateStatus.Hired),
                        Rejected = g.Count(x => x.Status == CandidateStatus.Rejected)
                    })
                    .OrderByDescending(x => x.Total)
                    .ToList()
            };

            return View(report);
        }

        private bool CandidateExists(int id)
        {
            return _context.Candidates.Any(e => e.Id == id);
        }
    }
}
