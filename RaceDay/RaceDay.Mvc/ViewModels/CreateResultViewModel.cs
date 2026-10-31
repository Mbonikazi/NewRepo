
using System;
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.ViewModels
{
    public class CreateResultViewModel
    {
        public int EnrolmentId { get; set; }

        public int EventId { get; set; }

        public string ParticipantName { get; set; } = string.Empty;

        public string EventName { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the finishing time.")]
        public TimeSpan FinishTime { get; set; }

        [Required(ErrorMessage = "Please enter the finishing position.")]
        [Range(1, 10000)]
        public int FinishingPosition { get; set; }
    }
}

