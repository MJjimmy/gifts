using GiftOfTheGivers.Data;
using GiftOfTheGivers.Models;
using GiftOfTheGivers.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GiftOfTheGivers.Controllers
{
    public class VolunteerController : Controller
    {
        private readonly AppDbContext _db;

        public VolunteerController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View(new VolunteerSignupViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(VolunteerSignupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var volunteer = new Volunteer
            {
                FullName = model.FullName.Trim(),
                Email = model.Email.Trim(),
                Phone = model.Phone.Trim(),
                Skills = string.IsNullOrWhiteSpace(model.Skills) ? null : model.Skills!.Trim(),
                Availability = model.Availability,
                AreaOfInterest = model.AreaOfInterest,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _db.Volunteers.Add(volunteer);
            await _db.SaveChangesAsync();

            TempData["Success"] =
                $"Thank you, {volunteer.FullName}! Your volunteer registration has been received. " +
                "Our team will contact you soon.";

            return RedirectToAction(nameof(Index));
        }
    }
}
