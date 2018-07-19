using System;
using HackneyRepairs.PropertyService;

namespace HackneyRepairs.Models
{
    public class PropertyDetails : PropertySummary
    {
        public bool Maintainable{ get; set; }
    }
}
