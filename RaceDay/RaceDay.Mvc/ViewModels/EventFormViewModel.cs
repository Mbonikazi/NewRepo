
using System;
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.ViewModels
{
    public class EventFormViewModel
    {
        [Required(ErrorMessage = "Please enter the event name.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the description.")]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select the event date.")]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Please enter the location.")]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        [Required]
        [Range(0.1, 500, ErrorMessage = "Distance must be between 0.1 and 500 km.")]
        public double DistanceKm { get; set; }

        [Required(ErrorMessage = "Please select the event type.")]
        [StringLength(50)]
        public string EventType { get; set; } = "Road";
    }
}

