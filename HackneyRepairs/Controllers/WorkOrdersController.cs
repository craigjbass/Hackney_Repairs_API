﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HackneyRepairs.Actions;
using HackneyRepairs.Factories;
using HackneyRepairs.Interfaces;
using HackneyRepairs.Models;
using HackneyRepairs.Repository;
using Microsoft.AspNetCore.Mvc;

namespace HackneyRepairs.Controllers
{
	[Produces("application/json")]
	[Route("/v1/work_orders")]
	public class WorkOrdersController : Controller
	{
		private IHackneyWorkOrdersService _workOrdersService;
		private ILoggerAdapter<WorkOrdersActions> _workOrderLoggerAdapter;
		private ILoggerAdapter<PropertyActions> _propertyLoggerAdapter;


		public WorkOrdersController(ILoggerAdapter<WorkOrdersActions> workOrderLoggerAdapter, IUhtRepository uhtRepository, IUhwRepository uhwRepository, IUHWWarehouseRepository uhWarehouseRepository)
		{
			_workOrderLoggerAdapter = workOrderLoggerAdapter;
			var workOrderServiceFactory = new HackneyWorkOrdersServiceFactory();
			_workOrdersService = workOrderServiceFactory.build(uhtRepository, uhwRepository, uhWarehouseRepository, _workOrderLoggerAdapter);
		}

		// GET Work Order 
		/// <summary>
		/// Retrieves a work order
		/// </summary>
		/// <param name="workOrderReference">Work order reference</param>
		/// <returns>A work order entity</returns>
		/// <response code="200">Returns the work order for the work order reference</response>
		/// <response code="404">If there is no work order for the given reference</response>   
		/// <response code="500">If any errors are encountered</response>
		[HttpGet("{workOrderReference}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(404)]
		[ProducesResponseType(500)]
        public async Task<JsonResult> GetWorkOrder(string workOrderReference)
		{
			var workOrdersActions = new WorkOrdersActions(_workOrdersService, _workOrderLoggerAdapter);
			UHWorkOrder result = new UHWorkOrder();
			try
			{
				result = await workOrdersActions.GetWorkOrder(workOrderReference);
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

        // GET Work Order by property reference 
        /// <summary>
        /// Returns all work orders for a property
        /// </summary>
        /// <param name="propertyReference">UH Property reference</param>
        /// <returns>A list of work order entities</returns>
        /// <response code="200">Returns a list of work orders for the property reference</response>
        /// <response code="400">If no parameter or a parameter different than propertyReference is passed</response>   
        /// <response code="404">If there are no work orders for the given property reference</response>   
        /// <response code="500">If any errors are encountered</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
		public async Task<JsonResult> GetWorkOrderByPropertyReference(string propertyReference)
        {
			if (propertyReference == null)
            {
                var error = new ApiErrorMessage
                {
                   developerMessage = "Bad Request - Missing parameter",
                   userMessage = @"Bad Request"
                };
                var jsonResponse = Json(error);
                jsonResponse.StatusCode = 400;
                return jsonResponse; 
            }

			var workOrdersActions = new WorkOrdersActions(_workOrdersService, _workOrderLoggerAdapter);
			var result = new List<UHWorkOrder>();
            try
            {
				result = (await workOrdersActions.GetWorkOrderByPropertyReference(propertyReference)).ToList();
                var json = Json(result);
                json.StatusCode = 200;
                return json;
            }
            catch (MissingWorkOrderException ex)
            {
                var error = new ApiErrorMessage
                {
                    developerMessage = ex.Message,
                    userMessage = @"Cannot find work orders for this property reference"
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

        // GET Notes for a Work Order 
        /// <summary>
        /// Returns all notes for a work order
        /// </summary>
        /// <param name="workOrderReference">Work order reference</param>
        /// <returns>A list of notes entities</returns>
        /// <response code="200">Returns a list of notes for a work order reference</response>
        /// <response code="404">If there is no notes found for the work order</response>   
        /// <response code="500">If any errors are encountered</response>
        [HttpGet("{workOrderReference}/notes")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<JsonResult> GetNotesForWorkOrder(string workOrderReference)
        {
			var workOrdersActions = new WorkOrdersActions(_workOrdersService, _workOrderLoggerAdapter);
            IEnumerable<Note> result = new List<Note>();
            try
            {
                result = await workOrdersActions.GetNotesByWorkOrderReference(workOrderReference);
                var json = Json(result);
                json.StatusCode = 200;
                return json;
            }
            catch (MissingNotesException ex)
            {
                var error = new ApiErrorMessage
                {
                    developerMessage = ex.Message,
                    userMessage = @"Cannot find notes."
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

        // GET A feed of work orders
        // <summary>
        // Returns a list of work orders with a work order reference greater than the parameter startId
        // </summary>
        /// <param name="startId">A work order reference, results will have a grater id than this parameter</param>
        /// <param name="resultSize">The maximum number of work orders returned. Default value is 50</param>
        /// <returns>A list of work orders</returns>
        /// <response code="200">Returns a list of work orders</response>
        /// <response code="400">If a parameter is invalid</response>   
        /// <response code="500">If any errors are encountered</response>
        [HttpGet("feed")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<JsonResult> getWorkOrderFeed(string startId, int resultSize = 0)
        {
            if (string.IsNullOrWhiteSpace(startId))
            {
                var error = new ApiErrorMessage
                {
                    developerMessage = "Missing parameter - startId",
                    userMessage = @"Bad Request"
                };
                var jsonResponse = Json(error);
                jsonResponse.StatusCode = 400;
                return jsonResponse;
            }

            try
            {
				var workOrdersActions = new WorkOrdersActions(_workOrdersService, _workOrderLoggerAdapter);
                var result = await workOrdersActions.GetWorkOrdersFeed(startId, resultSize);
                var jsonResponse = Json(result);
                jsonResponse.StatusCode = 200;
                return jsonResponse;
            }
            catch (Exception ex)
            {
                var error = new ApiErrorMessage
                {
                    developerMessage = ex.Message
                };

                JsonResult jsonResponse;
                if (ex is UhtRepositoryException || ex is UHWWarehouseRepositoryException)
                {
                    error.userMessage = "we had issues with connecting to the data source.";
                    jsonResponse = Json(error);
                }
                else
                {
                    error.userMessage = "We had issues processing your request.";
                    jsonResponse = Json(error);
                }

                jsonResponse.StatusCode = 500;
                return jsonResponse;
            }
        }
	}
}
