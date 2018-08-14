using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HackneyRepairs.Entities;
using HackneyRepairs.Interfaces;
using HackneyRepairs.Repository;

namespace HackneyRepairs.Actions
{
    public class WorkOrdersActions
    {
		IHackneyWorkOrdersService _workOrdersService;
		private readonly ILoggerAdapter<WorkOrdersActions> _logger;

		public WorkOrdersActions(IHackneyWorkOrdersService workOrdersService, ILoggerAdapter<WorkOrdersActions> logger)
        {
			_workOrdersService = workOrdersService;
			_logger = logger;
        }

		public async Task<WorkOrderEntity> GetWorkOrder(string workOrderReference)
		{
			_logger.LogInformation($"Finding work order details for reference: {workOrderReference}");
		    var result = await _workOrdersService.GetWorkOrder(workOrderReference);            
            if (result == null)
			{
				_logger.LogError($"Work order not found for reference: {workOrderReference}");
				throw new MissingWorkOrderException();
			}
			_logger.LogInformation($"Work order details returned for: {workOrderReference}");
			return result;
		}

        public async Task<IEnumerable<WorkOrderEntity>> GetWorkOrderByPropertyReference(string propertyId)
        {
            _logger.LogInformation($"Finding work order details for Id: {propertyId}");
            var result = await _workOrdersService.GetWorkOrderByPropertyReference(propertyId);
            if (((List<WorkOrderEntity>)result).Count == 0)
            {
                _logger.LogError($"Work order not found for Id: {propertyId}");
                throw new MissingWorkOrderException();
            }
            _logger.LogInformation($"Work order details returned for: {propertyId}");
            return result;
        }

		public async Task<IEnumerable<NotesEntity>> GetNotesByWorkOrderReference(string workOrderReference)
		{
			_logger.LogInformation($"Finding notes by work order: {workOrderReference}");
			var result = await _workOrdersService.GetNotesByWorkOrderReference(workOrderReference);
			if (((List<NotesEntity>)result).Count == 0)
            {
				_logger.LogError($"Notes not found for: {workOrderReference}");
                throw new MissingNotesException();
            }
			_logger.LogInformation($"Notes returned for: {workOrderReference}");
            return result;
		}
    }
    
    public class MissingWorkOrderException : Exception { }
	public class MissingNotesException : Exception { }
}
