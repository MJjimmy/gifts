using GiftOfTheGivers.Data;
using GiftOfTheGivers.Models;
using GiftOfTheGivers.Services;
using GiftOfTheGivers.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GiftOfTheGivers.Controllers
{
    [Authorize(Roles = "Employee")]
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly FunctionClient _functionClient;

        public EmployeeController(AppDbContext db, UserManager<AppUser> userManager, FunctionClient functionClient)
        {
            _db = db;
            _userManager = userManager;
            _functionClient = functionClient;
        }

        public async Task<IActionResult> Dashboard()
        {
            var model = new EmployeeDashboardViewModel
            {
                ActiveProjects = await _db.Projects.CountAsync(p => p.IsActive && p.Status == "Ongoing"),
                VolunteerCount = await _db.Volunteers.CountAsync(),
                PendingVolunteers = await _db.Volunteers.CountAsync(v => v.Status == "Pending"),
                UpdateCount = await _db.ProjectUpdates.CountAsync(),
                DonationCount = await _db.Donations.CountAsync(),
                TotalDonated = await _db.Donations.SumAsync(d => (decimal?)d.Amount) ?? 0m,
                OngoingProjects = await _db.Projects
                    .Where(p => p.IsActive && p.Status == "Ongoing")
                    .OrderByDescending(p => p.StartDate)
                    .Take(3)
                    .ToListAsync(),
                RecentDonations = await _db.Donations
                    .Include(d => d.Project)
                    .OrderByDescending(d => d.CreatedAt)
                    .Take(5)
                    .ToListAsync(),
                RecentVolunteers = await _db.Volunteers
                    .OrderByDescending(v => v.CreatedAt)
                    .Take(5)
                    .ToListAsync()
            };

            return View(model);
        }

        public async Task<IActionResult> Volunteers()
        {
            var volunteers = await _db.Volunteers
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();

            var model = new VolunteerManagementViewModel
            {
                Volunteers = volunteers,
                TotalCount = volunteers.Count,
                PendingCount = volunteers.Count(v => v.Status == "Pending"),
                ApprovedCount = volunteers.Count(v => v.Status == "Approved")
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetVolunteerStatus(int id, string status)
        {
            if (status is not ("Approved" or "Declined" or "Pending"))
            {
                return BadRequest();
            }

            var volunteer = await _db.Volunteers.FindAsync(id);
            if (volunteer == null)
            {
                return NotFound();
            }

            volunteer.Status = status;
            await _db.SaveChangesAsync();

            TempData["Success"] = $"{volunteer.FullName}'s registration was marked as {status.ToLowerInvariant()}.";
            return RedirectToAction(nameof(Volunteers));
        }

        [HttpGet]
        public async Task<IActionResult> PostUpdate()
        {
            await LoadProjectOptionsAsync();
            return View(new PostUpdateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostUpdate(PostUpdateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadProjectOptionsAsync();
                return View(model);
            }

            var project = await _db.Projects.FirstOrDefaultAsync(p => p.Id == model.ProjectId);
            if (project == null)
            {
                ModelState.AddModelError(nameof(model.ProjectId), "Please select a valid relief project.");
                await LoadProjectOptionsAsync();
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);

            var update = new ProjectUpdate
            {
                ProjectId = project.Id,
                Title = model.Title.Trim(),
                Body = model.Body.Trim(),
                PostedBy = string.IsNullOrWhiteSpace(user?.FullName) ? User.Identity?.Name ?? "Employee" : user!.FullName,
                CreatedAt = DateTime.UtcNow
            };

            if (model.ProgressPercent.HasValue)
            {
                project.ProgressPercent = model.ProgressPercent.Value;
            }

            _db.ProjectUpdates.Add(update);
            await _db.SaveChangesAsync();

            // Serverless automation: forward the update to the Azure Function so it
            // is logged in Azure Blob Storage. Silently skipped when offline.
            await _functionClient.LogProjectUpdateAsync(project, update);

            TempData["Success"] = $"Your update was posted to {project.Title}.";
            return RedirectToAction("Details", "Projects", new { id = project.Id });
        }

        private async Task LoadProjectOptionsAsync()
        {
            ViewBag.Projects = await _db.Projects
                .Where(p => p.IsActive)
                .OrderBy(p => p.Title)
                .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Title })
                .ToListAsync();
        }
    }
}
