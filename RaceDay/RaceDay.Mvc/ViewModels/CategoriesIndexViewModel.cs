
using System.Collections.Generic;
using RaceDay.Contracts;

namespace RaceDay.Mvc.ViewModels
{
    public class CategoriesIndexViewModel
    {
        public List<EventDto> Events { get; set; } = new();

        public List<CategoryDto> Categories { get; set; } = new();

        public int? SelectedEventId { get; set; }
    }
}
