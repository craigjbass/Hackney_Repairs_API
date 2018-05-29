using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DrsAppointmentsService;
using HackneyRepairs.Models;

namespace HackneyRepairs.Interfaces
{
    public interface IHackneyAppointmentsServiceRequestBuilder
    {
        xmbCheckAvailability BuildXmbCheckAvailabilityRequest(string workOrderReference, string sessionId, DrsOrder drsOrder, DateTime startPeriod, DateTime endPeriod);

        xmbOpenSession BuildXmbOpenSessionRequest();

        xmbCloseSession BuildXmbCloseSessionRequest(string sessionId);

        xmbCreateOrder BuildXmbCreateOrderRequest(string workOrderReference, string sessionId, DrsOrder drsOrder);

        xmbScheduleBooking BuildXmbScheduleBookingRequest(string workOrderReference, string sessionId, DateTime beginDate, DateTime endDate, DrsOrder drsOrder);

        xmbSelectOrder BuildXmbSelectOrderRequest(string workOrderReference, string sessionId);
    }
}
