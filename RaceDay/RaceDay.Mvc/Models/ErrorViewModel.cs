
using System;

namespace RaceDay.Mvc.Models
{
    /// <summary>
    /// Represents the information displayed on the application's
    /// error page when an unexpected error occurs.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the current request.
        /// This helps developers identify and investigate errors.
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// Gets a value indicating whether the request identifier
        /// should be displayed on the error page.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}

