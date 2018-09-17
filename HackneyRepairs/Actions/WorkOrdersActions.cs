using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HackneyRepairs.Entities;
using HackneyRepairs.Interfaces;
using HackneyRepairs.Models;
using HackneyRepairs.Repository;

namespace HackneyRepairs.Actions
{
    public class WorkOrdersActions
    {
		IHackneyWorkOrdersService _workOrdersService;
		IHackneyPropertyService _propertyService;
		private readonly ILoggerAdapter<WorkOrdersActions> _logger;

		public WorkOrdersActions(IHackneyWorkOrdersService workOrdersService, IHackneyPropertyService propertyService, ILoggerAdapter<WorkOrdersActions> logger)
        {
			_workOrdersService = workOrdersService;
			_propertyService = propertyService;
			_logger = logger;
        }

		public async Task<UHWorkOrder> GetWorkOrder(string workOrderReference)
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
        
		public async Task<IEnumerable<UHWorkOrder>> GetWorkOrderByPropertyReferences(string propertyId, bool childrenIncluded,
                                                                      string trade, string from, string to, string status)
        {
			// get all children properties for the given property id
			List<string> newIds = new List<string>();
            if (childrenIncluded)
            {
                List<PropertyLevelModel> currentChildren = new List<PropertyLevelModel>();
				currentChildren.Add(await _propertyService.GetPropertyLevelInfo(propertyId));
				newIds.Add(propertyId);
				while (currentChildren.Count() > 0)
				{
					List<PropertyLevelModel> newChildren = new List<PropertyLevelModel>();

					foreach (PropertyLevelModel prop in currentChildren)
					{
						newChildren.InsertRange(0, await _propertyService.GetPropertyLevelInfosForParent(prop.PropertyReference));
                    }
					if (newChildren.Count() == 0)
					{
						break;
					}
					newIds.InsertRange(0,(from x in newChildren select x.PropertyReference).ToList());
					currentChildren = newChildren.Where(c => c.LevelCode != "8").ToList();
                }
			}

            // get all work orders for all properties
            _logger.LogInformation($"Finding work order details for Id: {propertyId}");
			var result = await _workOrdersService.GetWorkOrderByPropertyReferences(newIds);
			if (((List<UHWorkOrder>)result).Count == 0)
            {
                _logger.LogError($"Work order not found for Id: {propertyId}");
                throw new MissingWorkOrderException();
            }
            _logger.LogInformation($"Work order details returned for: {propertyId}");
            return result;
        }

		public async Task<IEnumerable<Note>> GetNotesByWorkOrderReference(string workOrderReference)
		{
			_logger.LogInformation($"Finding notes by work order: {workOrderReference}");
			var result = await _workOrdersService.GetNotesByWorkOrderReference(workOrderReference);
			if (((List<Note>)result).Count == 0)
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
