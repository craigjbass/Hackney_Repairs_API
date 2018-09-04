﻿using System;
using System.Collections.Generic;
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

        public async Task<IEnumerable<DetailedNote>> GetNoteFeed(int startId, string noteTarget, int? size)
        {
            var results = await _workOrdersService.GetNoteFeed(startId, noteTarget, size);
            return results;
        }
    }
}