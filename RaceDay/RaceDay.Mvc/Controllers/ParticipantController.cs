
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using RaceDay.Contracts;
using RaceDay.Mvc.Services;
using RaceDay.Mvc.ViewModels;

namespace RaceDay.Mvc.Controllers
{
    public class ParticipantController : Controller
    {
        private readonly RaceDayApiClient _api;

        public ParticipantController(RaceDayApiClient api)
        {
            _api = api;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult MyEvents()
        {
            return RedirectToAction("Index", "Enrolments");
        }

        [HttpGet]
        public IActionResult MyResults()
        {
            return RedirectToAction("Index", "Results");
        }

        // GET: Participant/MyProfile
        [HttpGet]
        public async Task<IActionResult> MyProfile()
        {
            if (HttpContext.Session.GetString("Role") != "Participant")
                return RedirectToAction("Login", "Auth");

            var response = await _api.SendAsync(
                HttpMethod.Get,
                "api/profile/me");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Could not load your profile.";
                return RedirectToAction("Index");
            }

            var profile = await _api.ReadAsync<ProfileDto>(response);

            if (profile == null)
            {
                TempData["Error"] = "Profile information was not found.";
                return RedirectToAction("Index");
            }

            var model = new ProfileViewModel
            {
                UserId = profile.UserId,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                Email = profile.Email,
                Role = profile.Role,
                CreatedAt = profile.CreatedAt
            };

            return View(model);
        }

        // POST: Participant/MyProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MyProfile(ProfileViewModel model)
        {
            if (HttpContext.Session.GetString("Role") != "Participant")
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
                return View(model);

            var request = new UpdateProfileRequest
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email
            };

            var response = await _api.SendAsync(
                HttpMethod.Put,
                "api/profile/me",
                request);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    "",
                    await response.Content.ReadAsStringAsync());

                return View(model);
            }

            HttpContext.Session.SetString(
                "DisplayName",
                model.FirstName);

            TempData["Success"] = "Your profile was updated successfully.";

            return RedirectToAction("MyProfile");
        }
    }
}


