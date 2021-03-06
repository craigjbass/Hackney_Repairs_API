﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DrsAppointmentsService;
using HackneyRepairs.Actions;
using HackneyRepairs.Entities;
using HackneyRepairs.Interfaces;

namespace HackneyRepairs.Services
{
    public class HackneyAppointmentsService : IHackneyAppointmentsService
    {
        private readonly SOAPClient _client;
		private IUhtRepository _uhtRepository;
        private ILoggerAdapter<AppointmentActions> _logger;
		public HackneyAppointmentsService(ILoggerAdapter<AppointmentActions> logger, IUhtRepository uhtRepository)
        {
            _client = new SOAPClient();
			_uhtRepository = uhtRepository;
            _logger = logger;
        }

        public Task<checkAvailabilityResponse> GetAppointmentsForWorkOrderReference(xmbCheckAvailability checkAvailabilityRequest)
        {
            _logger.LogInformation($"HackneyAppointmentsService/GetAppointmentsForWorkOrderReference(): Sent request to upstream AppointmentServiceClient (Order Id: {checkAvailabilityRequest.theOrder.orderId})");
            var response = _client.checkAvailabilityAsync(checkAvailabilityRequest);
            _logger.LogInformation($"HackneyAppointmentsService/GetAppointmentsForWorkOrderReference(): Received response from upstream PropertyServiceClient (Order Id: {checkAvailabilityRequest.theOrder.orderId})");
            return response;
        }

        public Task<openSessionResponse> OpenSessionAsync(xmbOpenSession openSession)
        {
            _logger.LogInformation($"HackneyAppointmentsService/OpenSessionAsync(): Sent request to upstream AppointmentServiceClient (Id: {openSession.id})");
            var response = _client.openSessionAsync(openSession);
            _logger.LogInformation($"HackneyAppointmentsService/OpenSessionAsync(): Received response from upstream PropertyServiceClient (Id: {openSession.id})");
            return response;
        }

        public Task<closeSessionResponse> CloseSessionAsync(xmbCloseSession closeSession)
        {
            _logger.LogInformation($"HackneyAppointmentsService/CloseSessionAsync(): Sent request to upstream AppointmentServiceClient (Id: {closeSession.id})");
            var response = _client.closeSessionAsync(closeSession);
            _logger.LogInformation($"HackneyAppointmentsService/CloseSessionAsync(): Received response from upstream PropertyServiceClient (Id: {closeSession.id})");
            return response;
        }

        public Task<createOrderResponse> CreateWorkOrderAsync(xmbCreateOrder createOrder)
        {
            _logger.LogInformation($"HackneyAppointmentsService/CreateWorkOrderAsync(): Sent request to upstream AppointmentServiceClient (Order Id: {createOrder.theOrder.orderId})");
            var response = _client.createOrderAsync(createOrder);
            _logger.LogInformation($"HackneyAppointmentsService/CreateWorkOrderAsync(): Received response from upstream PropertyServiceClient (Order Id: {createOrder.theOrder.orderId})");
            return response;
        }

        public Task<scheduleBookingResponse> ScheduleBookingAsync(xmbScheduleBooking scheduleBooking)
        {
            _logger.LogInformation($"HackneyAppointmentsService/ScheduleBookingAsync(): Sent request to upstream AppointmentServiceClient (Order Id: {scheduleBooking.theBooking.orderId})");
            var response = _client.scheduleBookingAsync(scheduleBooking);
            _logger.LogInformation($"HackneyAppointmentsService/ScheduleBookingAsync(): Received response from upstream PropertyServiceClient (Order Id: {scheduleBooking.theBooking.orderId})");
            return response;
        }

        public Task<selectOrderResponse> SelectOrderAsync(xmbSelectOrder selectOrder)
        {
            _logger.LogInformation($"HackneyAppointmentsService/SelectOrderAsync(): Sent request to upstream AppointmentServiceClient (Order Id: {selectOrder.primaryOrderNumber})");
            var response = _client.selectOrderAsync(selectOrder);
            _logger.LogInformation($"HackneyAppointmentsService/SelectOrderAsync(): Received response from upstream PropertyServiceClient (Order Id: {selectOrder.primaryOrderNumber})");
            return response;
        }

        public Task<selectBookingResponse> SelectBookingAsync(xmbSelectBooking selectBooking)
        {
            _logger.LogInformation($"HackneyAppointmentsService/SelectBookingAsync(): Sent request to upstream AppointmentServiceClient (Id: {selectBooking.id})");
            var response = _client.selectBookingAsync(selectBooking);
            _logger.LogInformation($"HackneyAppointmentsService/SelectBookingAsync(): Received response from upstream PropertyServiceClient (Id: {selectBooking.id})");
            return response;
        }

		public Task<IEnumerable<UhtAppointmentEntity>> GetAppointmentsByWorkOrderReference (string workOrderReference)
        {
			_logger.LogInformation($"HackneyAppointmentsService/GetAppointmentsByWorkOrderReference(): Sent request to get appointments for workOrderReference: {workOrderReference})");
			var response = _uhtRepository.GetAppointmentsByWorkOrderReference(workOrderReference);
			_logger.LogInformation($"HackneyAppointmentsService/GetAppointmentsByWorkOrderReference(): Received response for workOrderReference: {workOrderReference})");
            return response;
		}
    }
}
