using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HackneyRepairs.Models;

namespace HackneyRepairs.Formatters
{
    public static class AppointmentFormatter
    {
        public static object FormatAppointment(this Appointment appointment)
        {
            return new
            {
                beginDate = DateTimeFormatter.FormatDateTimeToUtc(appointment.BeginDate),
                endDate = DateTimeFormatter.FormatDateTimeToUtc(appointment.EndDate),
            };
        }
    }
}
