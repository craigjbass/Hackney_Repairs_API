using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HackneyRepairs.Actions;
using HackneyRepairs.Entities;
using HackneyRepairs.Interfaces;
using HackneyRepairs.Models;
using RepairsService;

namespace HackneyRepairs.Services
{
	public class HackneyRepairsService : IHackneyRepairsService
	{
		private RepairServiceClient _client;
		private IUhtRepository _uhtRepository;
		private IUhwRepository _uhwRepository;
        private IUHWWarehouseRepository _uhWarehouseRepository;
		private ILoggerAdapter<RepairsActions> _logger;
        public HackneyRepairsService(IUhtRepository uhtRepository, IUhwRepository uhwRepository, IUHWWarehouseRepository uHWWarehouseRepository, ILoggerAdapter<RepairsActions> logger)
		{
			_client = new RepairServiceClient();
			_uhtRepository = uhtRepository;
			_uhwRepository = uhwRepository;
            _uhWarehouseRepository = uHWWarehouseRepository;
			_logger = logger;
		}

		public Task<IEnumerable<RepairRequestBase>> GetRepairByPropertyReference(string propertyReference)
        {
			_logger.LogInformation($"HackneyWorkOrdersService/GetWorkOrderByReference(): Sent request to UhtRepository (Property reference: {propertyReference})");
            var response = _uhtRepository.GetRepairRequests(propertyReference);
			_logger.LogInformation($"HackneyWorkOrdersService/GetWorkOrderByReference(): Work order details returned for property reference: {propertyReference})");
            return response;
        }

		public Task<RepairCreateResponse> CreateRepairAsync(NewRepairRequest request)
		{
			_logger.LogInformation($"HackneyRepairsService/CreateRepairAsync(): Sent request to upstream RepairServiceClient (Request ref: {request.RepairRequest.RequestReference})");
			var response = _client.CreateRepairRequestAsync(request);
			_logger.LogInformation($"HackneyRepairsService/CreateRepairAsync(): Received response from upstream RepairServiceClient (Request ref: {request.RepairRequest.RequestReference})");
			return response;
		}

		public Task<WorksOrderListResponse> CreateRepairWithOrderAsync(NewRepairTasksRequest repairRequest)
		{
			_logger.LogInformation($"HackneyRepairsService/CreateRepairWithOrderAsync(): Sent request to upstream RepairServiceClient (Request ref: {repairRequest.RepairRequest.RequestReference})");
			var response = _client.CreateRepairWithOrderAsync(repairRequest);
			_logger.LogInformation($"HackneyRepairsService/CreateRepairWithOrderAsync(): Received response from upstream RepairServiceClient (Request ref: {repairRequest.RepairRequest.RequestReference})");
			return response;
		}

        public async Task<DrsOrder> GetWorkOrderDetails(string workOrderReference)
		{
            _logger.LogInformation($"HackneyRepairsService/GetWorkOrderDetails(): Sent request to uhWarehouseRepository (Work order ref ref: {workOrderReference})");
            var warehouseResponse = await _uhWarehouseRepository.GetWorkOrderDetails(workOrderReference);
            if (warehouseResponse != null)
            {
                return warehouseResponse;
            }

            _logger.LogInformation($"HackneyRepairsService/GetWorkOrderDetails(): No response from uhWarehouseRepository, sent request to UHT (Work order ref: {workOrderReference})");
            var uhResponse = await _uhtRepository.GetWorkOrderDetails(workOrderReference);
            return uhResponse;
		}

		public Task<bool> UpdateRequestStatus(string repairRequestReference)
		{
			_logger.LogInformation($"HackneyRepairsService/UpdateRequestStatus(): Sent request to upstream RepairServiceClient (Repair request equest ref: {repairRequestReference})");
			var response = _uhtRepository.UpdateRequestStatus(repairRequestReference);
			_logger.LogInformation($"HackneyRepairsService/UpdateRequestStatus(): Received response from upstream RepairServiceClient (Repair request equest ref: {repairRequestReference})");
			return response;
		}

		public Task<TaskListResponse> GetRepairTasksAsync(RepairRefRequest request)
		{
			_logger.LogInformation($"HackneyRepairsService/GetRepairTasksAsync(): Sent request to upstream RepairServiceClient (Request ref: {request.RequestReference})");
			var response = _client.GetRepairTasksByRequestAsync(request);
			_logger.LogInformation($"HackneyRepairsService/GetRepairTasksAsync(): Received response from upstream RepairServiceClient (Request ref: {request.RequestReference})");
			return response;
		}

		public Task<RepairGetResponse> GetRepairRequestByReferenceAsync(RepairRefRequest request)
		{
			_logger.LogInformation($"HackneyRepairsService/GetRepairRequestByReferenceAsync(): Sent request to upstream RepairServiceClient (Request ref: {request.RequestReference})");
			var response = _client.GetRepairRequestByReferenceAsync(request);
			_logger.LogInformation($"HackneyRepairsService/GetRepairRequestByReferenceAsync(): Received response from upstream RepairServiceClient (Request ref: {request.RequestReference})");
			return response;
		}

		public Task<int?> UpdateUHTVisitAndBlockTrigger(string workOrderReference, DateTime startDate, DateTime endDate, int orderId, int bookingId, string slotDetail)
		{
			_logger.LogInformation($"HackneyRepairsService/UpdateUHTVisitAndBlockTrigger(): Sent request to upstream UHTdb (Work order ref : {workOrderReference})");
			var response = _uhtRepository.UpdateVisitAndBlockTrigger(workOrderReference, startDate, endDate, orderId, bookingId, slotDetail);
			_logger.LogInformation($"HackneyRepairsService/UpdateUHTVisitAndBlockTrigger(): Received response from upstream  UHTdb (Work order ref: {workOrderReference})");
			return response;
		}

		public Task<WebResponse> IssueOrderAsync(WorksOrderRequest request)
		{
			_logger.LogInformation($"HackneyRepairsService/IssueOrderAsync(): Sent request to upstream RepairServiceClient (Order ref: {request.OrderReference})");
			var response = _client.IssueOrderAsync(request);
			_logger.LogInformation($"HackneyRepairsService/IssueOrderAsync(): Received response from upstream RepairServiceClient (Order ref: {request.OrderReference})");
			return response;
		}

		public Task AddOrderDocumentAsync(string documentType, string workOrderReference, int workOrderId, string processComment)
		{
			_logger.LogInformation($"HackneyRepairsService/AddOrderDocumentAsync(): Sent request to upstream  UHTdb (Work order ref: {workOrderReference})");
			var response = _uhwRepository.AddOrderDocumentAsync(documentType, workOrderReference, workOrderId, processComment);
			_logger.LogInformation($"HackneyRepairsService/AddOrderDocumentAsync(): Received response from upstream  UHTdb (Work order ref: {workOrderReference})");
			return response;
		}
	}
}
