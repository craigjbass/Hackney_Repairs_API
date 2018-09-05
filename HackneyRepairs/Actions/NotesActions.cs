using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HackneyRepairs.Interfaces;
using HackneyRepairs.Models;

namespace HackneyRepairs.Actions
{
    public class NotesActions
    {
        private readonly ILoggerAdapter<NotesActions> _logger;
        private readonly IHackneyWorkOrdersService _workOrdersService;

        public NotesActions(IHackneyWorkOrdersService workOrdersService, ILoggerAdapter<NotesActions> logger)
        {
            _logger = logger;
            _workOrdersService = workOrdersService;
        }

        public async Task<IEnumerable<DetailedNote>> GetNoteFeed(int startId, string noteTarget, int size)
        {
            _logger.LogInformation($"Getting results for: {startId}");
            var results = await _workOrdersService.GetNoteFeed(startId, noteTarget, size);

            if (results.Count() == 1 && string.IsNullOrWhiteSpace(results.FirstOrDefault().WorkOrderReference))
            {
                throw new MissingNoteTargetException();
            }
            return results;
        }
    }

    public class MissingNoteTargetException : Exception {}
}
