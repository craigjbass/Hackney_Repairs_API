using System;
using System.Threading.Tasks;
using HackneyRepairs.Actions;
using HackneyRepairs.Entities;
using HackneyRepairs.Factories;
using HackneyRepairs.Interfaces;
using HackneyRepairs.Models;
using HackneyRepairs.Repository;
using Microsoft.AspNetCore.Mvc;

namespace HackneyRepairs.Controllers
{
	[Produces("application/json")]
	[Route("v1/workorders")]
	public class WorkOrdersController : Controller
	{
		private IHackneyWorkOrdersService _workOrdersService;
		private ILoggerAdapter<WorkOrdersActions> _loggerAdapter;

		public WorkOrdersController(ILoggerAdapter<WorkOrdersActions> loggerAdapter, IUhtRepository uhtRepository)
		{
			_loggerAdapter = loggerAdapter;
			var factory = new HackneyWorkOrdersServiceFactory();
			_workOrdersService = factory.build(uhtRepository, _loggerAdapter);
		}

		// GET Work Order 
		/// <summary>
		/// Retrieves a work order
		/// </summary>
		/// <param name="workOrderReference">Work order reference</param>
		/// <returns>A work order entity</returns>
		/// <response code="200">Returns the the work order for the work order reference</response>
		/// <response code="404">If there is no work order for the given reference</response>   
		/// <response code="500">If any errors are encountered</response>
		[HttpGet("{workOrderReference}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(404)]
		[ProducesResponseType(500)]
		public async Task<JsonResult> Get(string workOrderReference)
		{
			var workOrdersActions = new WorkOrdersActions(_workOrdersService, _loggerAdapter);
			WorkOrderEntity result = new WorkOrderEntity();
			try
			{
				result = await workOrdersActions.GetWorkOrderByReference(workOrderReference);
				var json = Json(result);
				json.StatusCode = 200;
				return json;
			}
			catch (MissingWorkOrderException ex)
			{
				var error = new ApiErrorMessage
				{
					developerMessage = ex.Message,
					userMessage = @"Cannot find work order."
				};
				var jsonResponse = Json(error);
				jsonResponse.StatusCode = 404;
				return jsonResponse;
			}
			catch (UhtRepositoryException ex)
			{
				var error = new ApiErrorMessage
				{
					developerMessage = ex.Message,
					userMessage = @"We had issues with connecting to the data source."
				};
				var jsonResponse = Json(error);
				jsonResponse.StatusCode = 500;
				return jsonResponse;
			}
		}
	}
}
