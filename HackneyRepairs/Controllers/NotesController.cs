using System;
using System.Threading.Tasks;
using HackneyRepairs.Actions;
using HackneyRepairs.Factories;
using HackneyRepairs.Interfaces;
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
        public async Task<JsonResult> GetFeedNotes(string startId, string noteTarget, int ResultSize)
        {
            //var noteActions = new NotesActions(_workOrdersService, _loggerAdapter);
            //return Json(await noteActions.GetNoteFeed(startId));
            throw new NotImplementedException();
        }
    }
}