using GiftOfTheGivers.Data;
using GiftOfTheGivers.Helpers;
using GiftOfTheGivers.Models;
using GiftOfTheGivers.Services;
using GiftOfTheGivers.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GiftOfTheGivers.Controllers
{
    public class DonationController : Controller
    {
        private readonly AppDbContext _db;
        private readonly FunctionClient _functionClient;

        public DonationController(AppDbContext db, FunctionClient functionClient)
        {
            _db = db;
            _functionClient = functionClient;
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
                Reference = DonationReferenceGenerator.Create(DateTime.UtcNow),
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

            // Serverless automation: the Azure Function generates the dummy tax
            // certificate. If the function app is not running, we fall back to
            // generating it locally so the flow never breaks.
            var certificate = await _functionClient.RequestTaxCertificateAsync(donation);
            TempData["CertificateSource"] = certificate is null ? "local" : "function";

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

            ViewBag.CertificateNumber = TaxCertificateNumberFormatter.Format(donation.Id, donation.CreatedAt);
            ViewBag.CertificateSource = TempData["CertificateSource"] as string ?? "local";

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
    }
}
