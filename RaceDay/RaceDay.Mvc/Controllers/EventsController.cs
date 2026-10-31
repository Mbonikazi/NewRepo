

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using RaceDay.Contracts;
using RaceDay.Mvc.Services;
using RaceDay.Mvc.ViewModels;

namespace RaceDay.Mvc.Controllers
{
    public class EventsController : Controller
    {
        private readonly RaceDayApiClient _api;

        public EventsController(RaceDayApiClient api)
        {
            _api = api;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var response = await _api.SendAsync(
                HttpMethod.Get, "api/events");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Could not load events.";
                return View(new List<EventDto>());
            }

            var events =
                await _api.ReadAsync<List<EventDto>>(response);

            return View(events ?? new List<EventDto>());
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _api.SendAsync(
                HttpMethod.Get, $"api/events/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Could not load event details.";
                return RedirectToAction(nameof(Index));
            }

            var eventDetails =
                await _api.ReadAsync<EventDto>(response);

            if (eventDetails == null)
                return RedirectToAction(nameof(Index));

            return View(eventDetails);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Auth");

            return View(new EventFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventFormViewModel vm)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
                return View(vm);

            var request = new CreateEventRequest
            {
                Name = vm.Name,
                Description = vm.Description,
                EventDate = vm.EventDate,
                Location = vm.Location,
                DistanceKm = vm.DistanceKm,
                EventType = vm.EventType
            };

            var response = await _api.SendAsync(
                HttpMethod.Post, "api/events", request);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("",
                    await response.Content.ReadAsStringAsync());
                return View(vm);
            }

            TempData["Success"] = "Event created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Auth");

            var response = await _api.SendAsync(
                HttpMethod.Get, $"api/events/{id}");

            if (!response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            var eventDetails =
                await _api.ReadAsync<EventDto>(response);

            if (eventDetails == null)
                return RedirectToAction(nameof(Index));

            var vm = new EventFormViewModel
            {
                Name = eventDetails.Name,
                Description = eventDetails.Description,
                EventDate = eventDetails.EventDate,
                Location = eventDetails.Location,
                DistanceKm = eventDetails.DistanceKm,
                EventType = eventDetails.EventType
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id, EventFormViewModel vm)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
                return View(vm);

            var request = new UpdateEventRequest
            {
                Name = vm.Name,
                Description = vm.Description,
                EventDate = vm.EventDate,
                Location = vm.Location,
                DistanceKm = vm.DistanceKm,
                EventType = vm.EventType
            };

            var response = await _api.SendAsync(
                HttpMethod.Put, $"api/events/{id}", request);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("",
                    await response.Content.ReadAsStringAsync());
                return View(vm);
            }

            TempData["Success"] = "Event updated successfully.";
            return RedirectToAction(nameof(Index));


        }

        // Display Delete confirmation page
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Auth");

            var response = await _api.SendAsync(
                HttpMethod.Get,
                $"api/events/{id}");

            if (!response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            var eventDetails =
                await _api.ReadAsync<EventDto>(response);

            if (eventDetails == null)
                return RedirectToAction(nameof(Index));

            return View(eventDetails);
        }

        // Confirm and delete event
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Auth");

            var response = await _api.SendAsync(
                HttpMethod.Delete,
                $"api/events/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Could not delete this event.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Event deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

    }
}

