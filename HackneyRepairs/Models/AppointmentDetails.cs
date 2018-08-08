using System;
namespace HackneyRepairs.Models
{
    public class AppointmentDetails : Appointment
    {
        public DateTime TargetDate { get; set; }
        public string AllocatedResource { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string DataSource { get; set; }
        // out of target attribute?
    }
}
