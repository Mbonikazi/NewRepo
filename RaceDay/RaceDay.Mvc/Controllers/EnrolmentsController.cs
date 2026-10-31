using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using RaceDay.Contracts;
using RaceDay.Mvc.Services;
using RaceDay.Mvc.ViewModels;

namespace RaceDay.Mvc.Controllers
{
    public class EnrolmentsController : Controller
    {
        private readonly RaceDayApiClient _api;

        public EnrolmentsController(RaceDayApiClient api)
        {
            _api = api;
        }

        // GET: Enrolments
        [HttpGet]
        public async Task<IActionResult> Index(int? eventId)
        {
            string? role = HttpContext.Session.GetString("Role");

            if (role != "Participant" && role != "Organiser")
                return RedirectToAction("Login", "Auth");

            var model = new EnrolmentsIndexViewModel
            {
                SelectedEventId = eventId
            };

            if (role == "Participant")
            {
                var response = await _api.SendAsync(
                    HttpMethod.Get, "api/enrolments/mine");

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Could not load your enrolments.";
                    return View(model);
                }

                model.Enrolments =
                    await _api.ReadAsync<List<EnrolmentDto>>(response)
                    ?? new List<EnrolmentDto>();

                return View(model);
            }

            var eventsResponse = await _api.SendAsync(
                HttpMethod.Get, "api/events/mine");

            if (!eventsResponse.IsSuccessStatusCode)
            {
                ViewBag.Error = "Could not load your events.";
                return View(model);
            }

            model.Events =
                await _api.ReadAsync<List<EventDto>>(eventsResponse)
                ?? new List<EventDto>();

            if (eventId.HasValue)
            {
                var enrolmentsResponse = await _api.SendAsync(
                    HttpMethod.Get,
                    $"api/enrolments/event/{eventId.Value}");

                if (enrolmentsResponse.IsSuccessStatusCode)
                {
                    model.Enrolments =
                        await _api.ReadAsync<List<EnrolmentDto>>(
                            enrolmentsResponse)
                        ?? new List<EnrolmentDto>();
                }
                else
                {
                    ViewBag.Error = "Could not load event enrolments.";
                }
            }

            return View(model);
        }

        // GET: Enrolments/Create
        [HttpGet]
        public async Task<IActionResult> Create(int eventId)
        {
            if (HttpContext.Session.GetString("Role") != "Participant")
                return RedirectToAction("Login", "Auth");

            var response = await _api.SendAsync(
                HttpMethod.Get,
                $"api/events/{eventId}/categories");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Could not load event categories.";
                return RedirectToAction("Index", "Events");
            }

            var categories =
                await _api.ReadAsync<List<CategoryDto>>(response)
                ?? new List<CategoryDto>();

            if (!categories.Any())
            {
                TempData["Error"] = "This event has no available categories.";
                return RedirectToAction("Details", "Events", new { id = eventId });
            }

            var model = new CreateEnrolmentViewModel
            {
                EventId = eventId,
                Categories = categories
            };

            return View(model);
        }

        // POST: Enrolments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CreateEnrolmentViewModel model)
        {
            if (HttpContext.Session.GetString("Role") != "Participant")
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
            {
                await LoadCategories(model);
                return View(model);
            }

            var request = new CreateEnrolmentRequest
            {
                EventId = model.EventId,
                CategoryId = model.CategoryId
            };

            var response = await _api.SendAsync(
                HttpMethod.Post,
                "api/enrolments",
                request);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    "",
                    await response.Content.ReadAsStringAsync());

                await LoadCategories(model);
                return View(model);
            }

            TempData["Success"] = "You have successfully enrolled in this event.";

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadCategories(CreateEnrolmentViewModel model)
        {
            var response = await _api.SendAsync(
                HttpMethod.Get,
                $"api/events/{model.EventId}/categories");

            if (response.IsSuccessStatusCode)
            {
                model.Categories =
                    await _api.ReadAsync<List<CategoryDto>>(response)
                    ?? new List<CategoryDto>();
            }
        }

        // POST: Enrolments/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(
            int id,
            int eventId,
            string status)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Auth");

            var request = new UpdateEnrolmentStatusRequest
            {
                Status = status
            };

            var response = await _api.SendAsync(
                HttpMethod.Put,
                $"api/enrolments/{id}/status",
                request);

            if (!response.IsSuccessStatusCode)
                TempData["Error"] = "Could not update enrolment status.";
            else
                TempData["Success"] = "Enrolment status updated successfully.";

            return RedirectToAction(nameof(Index), new { eventId });
        }
    }
}