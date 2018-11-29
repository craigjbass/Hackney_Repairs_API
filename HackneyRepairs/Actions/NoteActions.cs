using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HackneyRepairs.Interfaces;
using HackneyRepairs.Models;

namespace HackneyRepairs.Actions
{
    public class NoteActions
    {
        private readonly ILoggerAdapter<NoteActions> _logger;
        private readonly IHackneyWorkOrdersService _workOrdersService;
        private readonly IHackneyNotesService _notesService;

        public NoteActions(IHackneyWorkOrdersService workOrdersService, IHackneyNotesService notesService,  ILoggerAdapter<NoteActions> logger)
        {
            _logger = logger;
            _workOrdersService = workOrdersService;
            _notesService = notesService;
        }

        public async Task<IEnumerable<Note>> GetNoteFeed(int startId, string noteTarget, int size)
        {
            _logger.LogInformation($"Getting results for: {startId}");
            var results = await _workOrdersService.GetNoteFeed(startId, noteTarget, size);

            if (results.Count() == 1 && string.IsNullOrWhiteSpace(results.FirstOrDefault().WorkOrderReference))
            {
                throw new MissingNoteTargetException();
            }
            return results;
        }

        public async Task AddNote(NoteRequest note)
        {
            _logger.LogInformation($"Adding note for {note.ObjectKey} object for: {note.ObjectReference}");
            if (!await WorkOrderExists(note.ObjectReference))
            {
                throw new MissingWorkOrderException();
            }
            await _notesService.AddNote(note);
        }

        private async Task<bool> WorkOrderExists(string workOrderReference)
        {
            var workOrder = await _workOrdersService.GetWorkOrder(workOrderReference);
            return workOrder != null;
        }
    }

    public class MissingNoteTargetException : Exception {}
}
