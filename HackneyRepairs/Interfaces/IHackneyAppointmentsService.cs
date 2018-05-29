using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DrsAppointmentsService;

namespace HackneyRepairs.Interfaces
{
    public interface IHackneyAppointmentsService
    {
        Task<checkAvailabilityResponse> GetAppointmentsForWorkOrderReference(xmbCheckAvailability request);

        Task<openSessionResponse> OpenSessionAsync(xmbOpenSession openSession);

        Task<closeSessionResponse> CloseSessionAsync(xmbCloseSession closeSession);

        Task<createOrderResponse> CreateWorkOrderAsync(xmbCreateOrder createOrder);

        Task<scheduleBookingResponse> ScheduleBookingAsync(xmbScheduleBooking scheduleBooking);

        Task<selectOrderResponse> SelectOrderAsync(xmbSelectOrder selectOrder);
    }
}
