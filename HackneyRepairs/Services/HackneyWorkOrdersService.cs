using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HackneyRepairs.Actions;
using HackneyRepairs.Entities;
using HackneyRepairs.Interfaces;
using HackneyRepairs.Models;

namespace HackneyRepairs.Services
{
    public class HackneyWorkOrdersService : IHackneyWorkOrdersService
    {
        private IUhtRepository _uhtRepository;
		private IUhwRepository _uhwRepository;
        private ILoggerAdapter<WorkOrdersActions> _logger;

		public HackneyWorkOrdersService(IUhtRepository uhtRepository, IUhwRepository uhwRepository, ILoggerAdapter<WorkOrdersActions> logger)
        {
            _uhtRepository = uhtRepository;
			_uhwRepository = uhwRepository;
            _logger = logger;
        }

		public Task<IEnumerable<Note>> GetNotesByWorkOrderReference(string workOrderReference)
		{
			_logger.LogInformation($"HackneyWorkOrdersService/GetNotesByWorkOrderReference(): Sent request to UhtRepository (WorkOrder reference: {workOrderReference})");
			var response = _uhwRepository.GetNotesByWorkOrderReference(workOrderReference);
			_logger.LogInformation($"HackneyWorkOrdersService/GetNotesByWorkOrderReference(): Notes returned for: {workOrderReference})");
            return response;
		}

		public Task<UHWorkOrder> GetWorkOrder(string workOrderReference)
        {
            _logger.LogInformation($"HackneyWorkOrdersService/GetWorkOrderByReference(): Sent request to UhtRepository (WorkOrder reference: {workOrderReference})");
            var response = _uhtRepository.GetWorkOrder(workOrderReference);
            _logger.LogInformation($"HackneyWorkOrdersService/GetWorkOrderByReference(): Work order details returned for: {workOrderReference})");
            return response;
        }

		public Task<IEnumerable<UHWorkOrder>> GetWorkOrderByPropertyReference(string propertyReference)
        {
			_logger.LogInformation($"HackneyWorkOrdersService/GetWorkOrderByPropertyReference(): Sent request to UhtRepository (Property reference: {propertyReference})");
            var response = _uhtRepository.GetWorkOrderByPropertyReference(propertyReference);
			_logger.LogInformation($"HackneyWorkOrdersService/GetWorkOrderByPropertyReference(): Work orders returned for: {propertyReference})");
            return response;
        }
    }
}
