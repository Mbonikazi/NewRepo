using System.Collections.Generic;
using RaceDay.Contracts;

namespace RaceDay.Mvc.ViewModels
{
    public class EnrolmentsIndexViewModel
    {
        public List<EventDto> Events { get; set; } = new();

        public List<EnrolmentDto> Enrolments { get; set; } = new();

        public int? SelectedEventId { get; set; }
    }
}