using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HackneyRepairs.Models;

namespace HackneyRepairs.Interfaces
{
    public interface IDRSRepository
    {
		Task<DetailedAppointment> GetCurrentAppointmentByWorkOrderReference(string workOrderReference);
    }
}
