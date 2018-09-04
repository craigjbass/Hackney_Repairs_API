using System;
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
    [Route("/v1/notes")]
    public class NotesController : Controller
    {
        private IHackneyWorkOrdersService _workOrdersService;
        private ILoggerAdapter<NotesActions> _loggerAdapter;
        private ILoggerAdapter<WorkOrdersActions> _workOrdersLoggerAdapter;

        public NotesController(ILoggerAdapter<NotesActions> logger, ILoggerAdapter<WorkOrdersActions> workOrdersLogger, IUhtRepository uhtRepository, IUhwRepository uhwRepository, IUHWWarehouseRepository uhWarehouseRepository)
        {
            _workOrdersLoggerAdapter = workOrdersLogger;
            _loggerAdapter = logger;
            var factory = new HackneyWorkOrdersServiceFactory();
            _workOrdersService = factory.build(uhtRepository, uhwRepository, uhWarehouseRepository, _workOrdersLoggerAdapter);
        }

        [HttpGet("feed")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<JsonResult> GetFeedNotes(int startId, string noteTarget, int? resultSize)
        {
            if (startId < 1)
            {
                var error = new ApiErrorMessage
                {
                    developerMessage = "Invalid parameter - Please use a valid startId",
                    userMessage = @"Bad Request"
                };
                var jsonResponse = Json(error);
                jsonResponse.StatusCode = 400;
                return jsonResponse;   
            }
            if (string.IsNullOrWhiteSpace(noteTarget))
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
            
            var notesActions = new NotesActions(_workOrdersService, _loggerAdapter);
            try
            {
                var result = await notesActions.GetNoteFeed(startId, noteTarget, resultSize);
                var json = Json(result);
                json.StatusCode = 200;
                return json;
            }
            catch (UHWWarehouseRepositoryException ex)
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
            catch (UhwRepositoryException ex)
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
            catch (Exception ex)
            {
                var error = new ApiErrorMessage
                {
                    developerMessage = ex.Message,
                    userMessage = @"We had issues processing your request."
                };
                var jsonResponse = Json(error);
                jsonResponse.StatusCode = 500;
                return jsonResponse;
            }

        }
    }
}