using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Contracts;
using RaceDay.Mvc.Services;
using RaceDay.Mvc.ViewModels;

namespace RaceDay.Mvc.Controllers
{
    public class OrganiserController : Controller
    {
        private readonly RaceDayApiClient _api;

        public OrganiserController(RaceDayApiClient api)
        {
            _api = api;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
            {
                return RedirectToAction("Login", "Auth");
            }

            var model = new OrganiserDashboardViewModel
            {
                OrganiserName = HttpContext.Session.GetString("DisplayName") ?? "Organiser"
            };

            var eventsResponse = await _api.SendAsync(HttpMethod.Get, "api/events/mine");

            if (!eventsResponse.IsSuccessStatusCode)
            {
                TempData["Error"] = "Could not load your events.";
                return View(model);
            }

            var events = await _api.ReadAsync<List<EventDto>>(eventsResponse);
            events ??= new List<EventDto>();

            model.TotalEvents = events.Count;
            model.UpcomingEvents = events
                .Where(e => e.EventDate >= DateTime.Today)
                .OrderBy(e => e.EventDate)
                .Select(e => e.Name)
                .ToList();

            var participantIds = new HashSet<int>();

            foreach (var eventItem in events)
            {
                var enrolmentsResponse = await _api.SendAsync(HttpMethod.Get, $"api/enrolments/event/{eventItem.EventId}");
                if (!enrolmentsResponse.IsSuccessStatusCode) continue;

                var enrolments = await _api.ReadAsync<List<EnrolmentDto>>(enrolmentsResponse);
                if (enrolments == null) continue;

                foreach (var enrolment in enrolments)
                {
                    participantIds.Add(enrolment.ParticipantId);
                }
            }

            model.TotalParticipants = participantIds.Count;

            return View(model);
        }

        // ==========================================
        // CREATE EVENT (WITH AZURE BLOB UPLOAD)
        // ==========================================
        [HttpGet]
        public IActionResult CreateEvent()
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent(CreateEventRequest model, IFormFile bannerFile)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 1. Send event data to the API
            var response = await _api.SendAsync(HttpMethod.Post, "api/events", model);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to create event in the API.");
                return View(model);
            }

            // 2. Read the response to get the newly created EventId SAFELY
            using var jsonDoc = System.Text.Json.JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            int newEventId = 0;

            if (jsonDoc.RootElement.TryGetProperty("eventId", out var idPropLower))
            {
                newEventId = idPropLower.GetInt32();
            }
            else if (jsonDoc.RootElement.TryGetProperty("EventId", out var idPropUpper))
            {
                newEventId = idPropUpper.GetInt32();
            }

            // 3. If an image was uploaded, send it to the API
            if (bannerFile != null && bannerFile.Length > 0 && newEventId > 0)
            {
                await _api.UploadFileAsync($"api/events/{newEventId}/banner", bannerFile);
            }

            TempData["Success"] = "Event created successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
