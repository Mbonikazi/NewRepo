
using System.Collections.Generic;

namespace RaceDay.Mvc.ViewModels
{
    public class OrganiserDashboardViewModel
    {
        /// <summary>
        /// Gets or sets the name of the organiser who is logged in.
        /// </summary>
        public string OrganiserName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the total number of events created.
        /// </summary>
        public int TotalEvents { get; set; }

        /// <summary>
        /// Gets or sets the total number of participants.
        /// </summary>
        public int TotalParticipants { get; set; }

        /// <summary>
        /// Gets or sets the list of upcoming event names.
        /// </summary>
        public List<string> UpcomingEvents { get; set; }
            = new List<string>();
    }
}
