using HackneyRepairs.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using HackneyRepairs.Models;
using RepairsService;

namespace HackneyRepairs.Actions
{
	public class RepairsActions
	{
		public IHackneyRepairsService _repairsService;
		public IHackneyRepairsServiceRequestBuilder _requestBuilder;
		public ILoggerAdapter<RepairsActions> _logger;
		public RepairsActions(IHackneyRepairsService repairsService, IHackneyRepairsServiceRequestBuilder requestBuilder, ILoggerAdapter<RepairsActions> logger)
		{
			_repairsService = repairsService;
			_requestBuilder = requestBuilder;
			_logger = logger;
		}

		public async Task<object> CreateRepair(RepairRequest request)
		{
			if (request.WorkOrders != null)
			{
				return await CreateRepairWithOrder(request);
			}
			else
			{
				return await CreateRepairWithoutOrder(request);
			}
		}

		public async Task<object> GetRepairByReference(string reference)
		{
			_logger.LogInformation($"Getting repair by reference: {reference}");
			var request = _requestBuilder.BuildRepairRequest(reference);
			var response = await _repairsService.GetRepairRequestByReferenceAsync(request);
			if (!response.Success)
			{
				throw new RepairsServiceException();
			}
			var repairResponse = response.RepairRequest;
			if (repairResponse == null)
			{
				throw new MissingRepairException();
			}
			var tasksListResponse = await GetRepairTasksList(reference);
			var tasksList = tasksListResponse.TaskList;
			if (tasksList != null)
			{
				return new
				{
					repairRequestReference = repairResponse.Reference.Trim(),
					problemDescription = repairResponse.Problem.Trim(),
					priority = repairResponse.PriorityCode.Trim(),
					propertyReference = repairResponse.PropertyReference.Trim(),
					contact = new { name = repairResponse.Name.Trim() },
					workOrders = tasksList.Select(s => new
					{
						workOrderReference = s.WorksOrderReference.Trim(),
						sorCode = s.JobCode.Trim(),
						supplierReference = s.SupplierReference.Trim()
					})
				};
			}
			return new
			{
				repairRequestReference = repairResponse.Reference.Trim(),
				problemDescription = repairResponse.Problem.Trim(),
				priority = repairResponse.PriorityCode.Trim(),
				propertyReference = repairResponse.PropertyReference.Trim(),
				contact = new { name = repairResponse.Name.Trim() }
			};
		}

		private async Task<object> CreateRepairWithOrder(RepairRequest request)
		{
			_logger.LogInformation($"Creating repair with order (prop ref: {request.PropertyReference})");
			var repairRequest = _requestBuilder.BuildNewRepairTasksRequest(request);

			var response = await _repairsService.CreateRepairWithOrderAsync(repairRequest);

			if (!response.Success)
			{
				throw new RepairsServiceException();
			}
			var workOrderList = response.WorksOrderList;
			if (workOrderList == null)
			{
				throw new MissingRepairRequestException();
			}
			var workOrderItem = workOrderList.FirstOrDefault();
			// update the request status to 000
			_repairsService.UpdateRequestStatus(workOrderItem.RepairRequestReference.Trim());

			var repairTasksResponse = await GetRepairTasksList(workOrderItem.RepairRequestReference);
			var tasksList = repairTasksResponse.TaskList;
			return new
			{
				repairRequestReference = workOrderItem.RepairRequestReference.Trim(),
				propertyReference = workOrderItem.PropertyReference.Trim(),
				problemDescription = request.ProblemDescription.Trim(),
				priority = request.Priority.Trim(),
				contact = new { name = request.Contact.Name, telephoneNumber = request.Contact.TelephoneNumber },
				workOrders = tasksList.Select(s => new
				{
					workOrderReference = s.WorksOrderReference.Trim(),
					sorCode = s.JobCode.Trim(),
					supplierReference = s.SupplierReference.Trim()
				}).ToArray()
			};
		}

		private async Task<object> CreateRepairWithoutOrder(RepairRequest request)
		{
			_logger.LogInformation($"Creating repair with no work order");
			var repairRequest = _requestBuilder.BuildNewRepairRequest(request);

			var response = await _repairsService.CreateRepairAsync(repairRequest);

			if (!response.Success)
			{
				throw new RepairsServiceException();
			}
			var repairResponse = response.RepairRequest;
			if (repairResponse == null)
			{
				throw new MissingRepairRequestException();
			}
			// update the request status to 000
			_repairsService.UpdateRequestStatus(repairResponse.Reference.Trim());

			return new
			{
				repairRequestReference = repairResponse.Reference.Trim(),
				problemDescription = repairResponse.Problem.Trim(),
				priority = repairResponse.PriorityCode.Trim(),
				propertyReference = repairResponse.PropertyReference.Trim(),
				contact = new { name = repairResponse.Name, telephoneNumber = request.Contact.TelephoneNumber }
			};
		}

		private async Task<TaskListResponse> GetRepairTasksList(string requestReference)
		{
			_logger.LogInformation($"Getting repair task list ({requestReference})");
			var request = _requestBuilder.BuildRepairRequest(requestReference);
			var response = await _repairsService.GetRepairTasksAsync(request);
			if (!response.Success)
			{
				throw new RepairsServiceException();
			}
			return response;
		}
	}

	public class MissingRepairRequestException : Exception
	{
	}

	public class RepairsServiceException : Exception
	{
	}
	public class MissingRepairException : Exception
	{
	}
}
