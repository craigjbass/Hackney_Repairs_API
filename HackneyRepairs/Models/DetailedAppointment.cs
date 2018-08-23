using System;
namespace HackneyRepairs.Models
{
	public class DetailedAppointment : Appointment
    {
		public string Status { get; set; }
		public string AssignedWorker { get; set; }
		public string Mobilephone { get; set; }
		public string Priority { get; set; }
		public string SourceSystem { get; set; }
    }
}
