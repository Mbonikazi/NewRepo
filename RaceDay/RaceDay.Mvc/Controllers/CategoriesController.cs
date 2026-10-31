using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using RaceDay.Contracts;
using RaceDay.Mvc.Services;
using RaceDay.Mvc.ViewModels;

namespace RaceDay.Mvc.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly RaceDayApiClient _api;

        public CategoriesController(RaceDayApiClient api)
        {
            _api = api;
        }

        // GET: Categories
        [HttpGet]
        public async Task<IActionResult> Index(int? eventId)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
            {
                return RedirectToAction("Login", "Auth");
            }

            var model = new CategoriesIndexViewModel
            {
                SelectedEventId = eventId
            };

            var eventsResponse = await _api.SendAsync(
                HttpMethod.Get,
                "api/events");

            if (!eventsResponse.IsSuccessStatusCode)
            {
                ViewBag.Error = "Could not load events.";
                return View(model);
            }

            model.Events =
                await _api.ReadAsync<List<EventDto>>(eventsResponse)
                ?? new List<EventDto>();

            if (eventId.HasValue)
            {
                var categoriesResponse = await _api.SendAsync(
                    HttpMethod.Get,
                    $"api/events/{eventId.Value}/categories");

                if (categoriesResponse.IsSuccessStatusCode)
                {
                    model.Categories =
                        await _api.ReadAsync<List<CategoryDto>>(categoriesResponse)
                        ?? new List<CategoryDto>();
                }
                else
                {
                    ViewBag.Error = "Could not load categories.";
                }
            }

            return View(model);
        }

        // GET: Categories/Create
        [HttpGet]
        public IActionResult Create(int eventId)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.EventId = eventId;

            return View(new CreateCategoryRequest
            {
                MinAge = 0,
                MaxAge = 100,
                EntryFee = 0
            });
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            int eventId,
            CreateCategoryRequest request)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.EventId = eventId;
                return View(request);
            }

            if (request.MinAge > request.MaxAge)
            {
                ModelState.AddModelError(
                    "MaxAge",
                    "Maximum age must be greater than or equal to minimum age.");

                ViewBag.EventId = eventId;
                return View(request);
            }

            var response = await _api.SendAsync(
                HttpMethod.Post,
                $"api/events/{eventId}/categories",
                request);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    "",
                    await response.Content.ReadAsStringAsync());

                ViewBag.EventId = eventId;
                return View(request);
            }

            TempData["Success"] = "Category created successfully.";

            return RedirectToAction(
                nameof(Index),
                new { eventId = eventId });
        }

        // GET: Categories/Edit
        [HttpGet]
        public async Task<IActionResult> Edit(int id, int eventId)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
            {
                return RedirectToAction("Login", "Auth");
            }

            var response = await _api.SendAsync(
                HttpMethod.Get,
                $"api/events/{eventId}/categories");

            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index), new { eventId });
            }

            var categories =
                await _api.ReadAsync<List<CategoryDto>>(response);

            var category = categories?.FirstOrDefault(
                c => c.CategoryId == id);

            if (category == null)
            {
                TempData["Error"] = "Category not found.";
                return RedirectToAction(nameof(Index), new { eventId });
            }

            var request = new CreateCategoryRequest
            {
                Name = category.Name,
                Description = category.Description,
                MinAge = category.MinAge,
                MaxAge = category.MaxAge,
                EntryFee = category.EntryFee
            };

            ViewBag.EventId = eventId;
            ViewBag.CategoryId = id;

            return View(request);
        }

        // POST: Categories/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            int eventId,
            CreateCategoryRequest request)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.EventId = eventId;
                ViewBag.CategoryId = id;
                return View(request);
            }

            if (request.MinAge > request.MaxAge)
            {
                ModelState.AddModelError(
                    "MaxAge",
                    "Maximum age must be greater than or equal to minimum age.");

                ViewBag.EventId = eventId;
                ViewBag.CategoryId = id;
                return View(request);
            }

            var response = await _api.SendAsync(
                HttpMethod.Put,
                $"api/categories/{id}",
                request);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Could not update category.";
            }
            else
            {
                TempData["Success"] = "Category updated successfully.";
            }

            return RedirectToAction(nameof(Index), new { eventId });
        }

        // POST: Categories/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int eventId)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
            {
                return RedirectToAction("Login", "Auth");
            }

            var response = await _api.SendAsync(
                HttpMethod.Delete,
                $"api/categories/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Could not delete category.";
            }
            else
            {
                TempData["Success"] = "Category deleted successfully.";
            }

            return RedirectToAction(nameof(Index), new { eventId });
        }
    }
}
