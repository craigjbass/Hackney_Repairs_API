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
        private ILoggerAdapter<NotesActions> _notesLoggerAdapter;
        private ILoggerAdapter<WorkOrdersActions> _workOrdersLoggerAdapter;

        public NotesController(ILoggerAdapter<NotesActions> logger, ILoggerAdapter<WorkOrdersActions> workOrdersLogger, IUhtRepository uhtRepository, IUhwRepository uhwRepository, IUHWWarehouseRepository uhWarehouseRepository)
        {
            _workOrdersLoggerAdapter = workOrdersLogger;
            _notesLoggerAdapter = logger;
            var factory = new HackneyWorkOrdersServiceFactory();
            _workOrdersService = factory.build(uhtRepository, uhwRepository, uhWarehouseRepository, _workOrdersLoggerAdapter);
        }

        // GET A feed of notes
        /// <summary>
        /// Returns a list of notes matching the noteTarget and with a note id greater than startId
        /// </summary>
        /// <param name="startId">A note id, results will have a grater id than this parameter</param>
        /// <param name="noteTarget">The kind of note defined in Universal housing</param>
        /// <param name="resultSize">The number of notes returned. Default value is 50</param>
        /// <returns>A list of notes</returns>
        /// <response code="200">Returns a list of notes</response>
        /// <response code="400">If a parameter is invalid</response>   
        /// <response code="404">If the noteTarget parameter does not exist un Universal Housing</response>   
        /// <response code="500">If any errors are encountered</response>
        [HttpGet("feed")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<JsonResult> GetFeedNotes(int startId, string noteTarget, int resultSize = 0)
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
            
            var notesActions = new NotesActions(_workOrdersService, _notesLoggerAdapter);
            try
            {
                var result = await notesActions.GetNoteFeed(startId, noteTarget, resultSize);
                var json = Json(result);
                json.StatusCode = 200;
                return json;
            }
            catch (Exception ex)
            {
                var error = new ApiErrorMessage
                {
                    developerMessage = ex.Message
                };

                JsonResult jsonResponse;
                if (ex is MissingNoteTargetException)
                {
                    error.userMessage = "noteTarget parameter does not exist in the data source";
                    jsonResponse = Json(error);
                    jsonResponse.StatusCode = 404;
                }
                else if (ex is UHWWarehouseRepositoryException || ex.InnerException is UhwRepositoryException)
                {
                    error.userMessage = "We had issues with connecting to the data source.";
                    jsonResponse = Json(error);
                    jsonResponse.StatusCode = 500;
                }
                else
                {
                    error.userMessage = "We had issues processing your request.";
                    jsonResponse = Json(error);
                    jsonResponse.StatusCode = 500;
                }
                return jsonResponse;
            }
        }
    }
}