using GiftOfTheGivers.Data;
using GiftOfTheGivers.Models;
using GiftOfTheGivers.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GiftOfTheGivers.Controllers
{
    public class DonationController : Controller
    {
        private readonly AppDbContext _db;

        public DonationController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            await LoadProjectOptionsAsync();
            return View();
        }

        // Receives prefills from the quick-donation form on the landing page.
        [HttpGet]
        public async Task<IActionResult> Checkout(
            int? project,
            string? amount,
            decimal? customAmount,
            string? currency,
            string? donationType,
            string? firstName,
            string? lastName,
            string? email,
            bool anonymous = false)
        {
            var model = new DonationCheckoutViewModel
            {
                ProjectId = project,
                Currency = string.IsNullOrWhiteSpace(currency) ? "ZAR" : currency.Trim().ToUpperInvariant(),
                DonationType = string.Equals(donationType, "Recurring", StringComparison.OrdinalIgnoreCase)
                    ? "Recurring"
                    : "OneTime",
                DonorName = string.Join(' ', new[] { firstName, lastName }
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => s!.Trim())),
                Email = email?.Trim() ?? string.Empty,
                IsAnonymous = anonymous
            };

            if (customAmount is > 0)
            {
                model.Amount = customAmount.Value;
            }
            else if (decimal.TryParse(amount, out var preset) && preset > 0)
            {
                model.Amount = preset;
            }

            await LoadProjectOptionsAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(DonationCheckoutViewModel model)
        {
            // Anonymous donors do not have to provide a name.
            if (!model.IsAnonymous && string.IsNullOrWhiteSpace(model.DonorName))
            {
                ModelState.AddModelError(nameof(model.DonorName),
                    "Please enter your full name or choose to donate anonymously.");
            }

            if (!ModelState.IsValid)
            {
                await LoadProjectOptionsAsync();
                return View(model);
            }

            var donation = new Donation
            {
                Reference = GenerateReference(),
                DonationType = model.DonationType,
                Amount = model.Amount,
                Currency = model.Currency,
                DonorName = model.IsAnonymous ? null : model.DonorName!.Trim(),
                Email = model.Email.Trim(),
                Phone = string.IsNullOrWhiteSpace(model.Phone) ? null : model.Phone!.Trim(),
                IsAnonymous = model.IsAnonymous,
                ProjectId = model.ProjectId,
                Message = string.IsNullOrWhiteSpace(model.Message) ? null : model.Message!.Trim(),
                Status = "Received",
                CreatedAt = DateTime.UtcNow
            };

            _db.Donations.Add(donation);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Confirmation), new { reference = donation.Reference });
        }

        [HttpGet]
        public async Task<IActionResult> Confirmation(string? reference)
        {
            if (string.IsNullOrWhiteSpace(reference))
            {
                return RedirectToAction(nameof(Index));
            }

            var donation = await _db.Donations
                .Include(d => d.Project)
                .FirstOrDefaultAsync(d => d.Reference == reference);

            if (donation == null)
            {
                return NotFound();
            }

            return View(donation);
        }

        /// <summary>Loads the project dropdown options into ViewBag.Projects.</summary>
        private async Task LoadProjectOptionsAsync()
        {
            var projects = await _db.Projects
                .Where(p => p.IsActive)
                .OrderBy(p => p.Title)
                .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Title })
                .ToListAsync();

            projects.Insert(0, new SelectListItem { Value = "", Text = "General Relief Fund" });

            ViewBag.Projects = projects;
        }

        /// <summary>Generates a public donation reference, e.g. GTG-20260924-K7QX2M.</summary>
        private static string GenerateReference()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var suffix = new string(Enumerable.Range(0, 6)
                .Select(_ => chars[Random.Shared.Next(chars.Length)])
                .ToArray());

            return $"GTG-{DateTime.UtcNow:yyyyMMdd}-{suffix}";
        }
    }
}
