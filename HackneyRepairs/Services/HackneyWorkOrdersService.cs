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
		private IUHWWarehouseRepository _uhWarehouseRepository;
        private ILoggerAdapter<WorkOrdersActions> _logger;

		public HackneyWorkOrdersService(IUhtRepository uhtRepository, IUhwRepository uhwRepository, IUHWWarehouseRepository uhWarehouseRepository, ILoggerAdapter<WorkOrdersActions> logger)
        {
            _uhtRepository = uhtRepository;
			_uhwRepository = uhwRepository;
			_uhWarehouseRepository = uhWarehouseRepository;
            _logger = logger;
        }

		public async Task<IEnumerable<Note>> GetNotesByWorkOrderReference(string workOrderReference)
		{
			_logger.LogInformation($"HackneyWorkOrdersService/GetNotesByWorkOrderReference(): Sent request to UhtRepository (WorkOrder reference: {workOrderReference})");
			var response = await _uhwRepository.GetNotesByWorkOrderReference(workOrderReference);
			_logger.LogInformation($"HackneyWorkOrdersService/GetNotesByWorkOrderReference(): Notes returned for: {workOrderReference})");
            return response;
		}

		public async Task<UHWorkOrder> GetWorkOrder(string workOrderReference)
        {
            _logger.LogInformation($"HackneyWorkOrdersService/GetWorkOrderByReference(): Sent request to UhtRepository (WorkOrder reference: {workOrderReference})");
            var response = await _uhtRepository.GetWorkOrder(workOrderReference);
            _logger.LogInformation($"HackneyWorkOrdersService/GetWorkOrderByReference(): Work order details returned for: {workOrderReference})");
            return response;
        }
        
		public async Task<IEnumerable<UHWorkOrder>> GetWorkOrderByPropertyReference(string propertyReference)
        {
			_logger.LogInformation($"HackneyWorkOrdersService/GetWorkOrderByPropertyReference(): Sent request to _UhtRepository to get data from live (Property reference: {propertyReference})");
            var liveData = await _uhtRepository.GetWorkOrderByPropertyReference(propertyReference);
			var lLiveData = (List<UHWorkOrder>)liveData;
			_logger.LogInformation($"HackneyWorkOrdersService/GetWorkOrderByPropertyReference(): {lLiveData.Count} work orders returned for: {propertyReference})");

			_logger.LogInformation($"HackneyWorkOrdersService/GetWorkOrderByPropertyReference(): Sent request to _UHWarehouseRepository to get data from warehouse (Property reference: {propertyReference})");
            var warehouseData =  await _uhWarehouseRepository.GetWorkOrderByPropertyReference(propertyReference);
			var lWarehouseData = (List<UHWorkOrder>)warehouseData;
			_logger.LogInformation($"HackneyWorkOrdersService/GetWorkOrderByPropertyReference(): {lWarehouseData.Count} work orders returned for: {propertyReference})");

			_logger.LogInformation($"HackneyWorkOrdersService/GetWorkOrderByPropertyReference(): Merging list from repositories to a single list");
			List<UHWorkOrder> result = lLiveData;
			result.InsertRange(0,lWarehouseData);
			_logger.LogInformation($"HackneyWorkOrdersService/GetWorkOrderByPropertyReference(): Total {result.Count} ork orders returned for: {propertyReference})");
			return result;
        }

        public async Task<IEnumerable<DetailedNote>> GetRecentNotes(string noteId)
        {
            // uhw and data warehouse query using cuttoff date.
            throw new NotImplementedException();
        }
    }
}
