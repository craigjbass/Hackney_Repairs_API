using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HackneyRepairs.Models;
using HackneyRepairs.Validators;
using HackneyRepairs.Models;

namespace HackneyRepairs.Interfaces
{
    public interface IScheduleBookingRequestValidator
    {
        ValidationResult Validate(string workOrderReference, ScheduleAppointmentRequest request);
    }
}
