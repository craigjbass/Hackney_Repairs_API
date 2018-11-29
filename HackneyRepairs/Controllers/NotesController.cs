using System;
using System.Threading.Tasks;
using HackneyRepairs.Actions;
using HackneyRepairs.Builders;
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
        private readonly IExceptionLogger _sentryLogger;

        public NotesController(ILoggerAdapter<NotesActions> logger, ILoggerAdapter<WorkOrdersActions> workOrdersLogger, IUhtRepository uhtRepository, IUhwRepository uhwRepository, IUHWWarehouseRepository uhWarehouseRepository, IExceptionLogger sentryLogger)
        
        {
            _workOrdersLoggerAdapter = workOrdersLogger;
            _notesLoggerAdapter = logger;
            var factory = new HackneyWorkOrdersServiceFactory();
            _workOrdersService = factory.build(uhtRepository, uhwRepository, uhWarehouseRepository, _workOrdersLoggerAdapter);
            _sentryLogger = sentryLogger;
        }

        // GET A feed of notes
        /// <summary>
        /// Returns a list of notes matching the noteTarget and with a note id greater than startId
        /// </summary>
        /// <param name="startId">A note id, results will have a grater id than this parameter</param>
        /// <param name="noteTarget">The kind of note defined in Universal Housing (required)</param>
        /// <param name="resultSize">The maximum number of notes returned. Default value is 50</param>
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
                return ResponseBuilder.Error(400, "Invalid parameter - Please use a valid startId", "Invalid parameter - Please use a valid startId");
            }
            if (string.IsNullOrWhiteSpace(noteTarget))
            {
                return ResponseBuilder.Error(400, "Missing parameter - notetarget", "Missing parameter - notetarget");
            }
            
            var notesActions = new NotesActions(_workOrdersService, _notesLoggerAdapter);
            try
            {
                var result = await notesActions.GetNoteFeed(startId, noteTarget, resultSize);
                return ResponseBuilder.Ok(result);
            }
            catch (Exception ex)
            {
                _sentryLogger.CaptureException(ex);
                if (ex is MissingNoteTargetException)
                {
                    var userMessage = "noteTarget parameter does not exist in the data source";
                    return ResponseBuilder.Error(404, userMessage, ex.Message);
                }
                if (ex is UHWWarehouseRepositoryException || ex is UhwRepositoryException)
                {
                    var userMessage = "We had issues with connecting to the data source.";
                    return ResponseBuilder.Error(500, userMessage, ex.Message);
                }
                return ResponseBuilder.Error(500, "We had issues processing your request.", ex.Message);
            }
        }
    }
}