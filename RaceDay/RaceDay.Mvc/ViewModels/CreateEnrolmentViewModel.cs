using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RaceDay.Contracts;

namespace RaceDay.Mvc.ViewModels
{
    public class CreateEnrolmentViewModel
    {
        public int EventId { get; set; }

        [Required(ErrorMessage = "Please select a category.")]
        public int CategoryId { get; set; }

        public string EventName { get; set; } = string.Empty;

        public List<CategoryDto> Categories { get; set; } = new();
    }
}