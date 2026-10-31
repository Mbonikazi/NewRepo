
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using RaceDay.Contracts;
using RaceDay.Mvc.Services;
using RaceDay.Mvc.ViewModels;

namespace RaceDay.Mvc.Controllers
{
    public class ResultsController : Controller
    {
        private readonly RaceDayApiClient _api;

        public ResultsController(RaceDayApiClient api)
        {
            _api = api;
        }

        // GET: Results/Index - Participant My Results
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Role") != "Participant")
                return RedirectToAction("Login", "Auth");

            var response = await _api.SendAsync(
                HttpMethod.Get,
                "api/results/mine");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Could not load your results.";
                return View(new List<ResultDto>());
            }

            var results = await _api.ReadAsync<List<ResultDto>>(response);

            return View(results ?? new List<ResultDto>());
        }

        // GET: Results/Create
        [HttpGet]
        public async Task<IActionResult> Create(int enrolmentId, int eventId)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Auth");

            var response = await _api.SendAsync(
                HttpMethod.Get,
                $"api/enrolments/event/{eventId}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Could not load the event enrolments.";
                return RedirectToAction(
                    "Index", "Enrolments", new { eventId });
            }

            var enrolments = await _api.ReadAsync<List<EnrolmentDto>>(response);

            var enrolment = enrolments?.FirstOrDefault(
                e => e.EnrolmentId == enrolmentId);

            if (enrolment == null || enrolment.Status != "Confirmed")
            {
                TempData["Error"] =
                    "Only confirmed enrolments can have results recorded.";

                return RedirectToAction(
                    "Index", "Enrolments", new { eventId });
            }

            var model = new CreateResultViewModel
            {
                EnrolmentId = enrolment.EnrolmentId,
                EventId = enrolment.EventId,
                ParticipantName = enrolment.ParticipantName,
                EventName = enrolment.EventName,
                CategoryName = enrolment.CategoryName
            };

            return View(model);
        }

        // POST: Results/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateResultViewModel model)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
                return View(model);

            var request = new CreateResultRequest
            {
                EnrolmentId = model.EnrolmentId,
                FinishTime = model.FinishTime,
                FinishingPosition = model.FinishingPosition
            };

            var response = await _api.SendAsync(
                HttpMethod.Post,
                "api/results",
                request);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    "",
                    await response.Content.ReadAsStringAsync());

                return View(model);
            }

            TempData["Success"] = "Race result recorded successfully.";

            return RedirectToAction(
                "Index", "Enrolments", new { eventId = model.EventId });
        }
    }
}


