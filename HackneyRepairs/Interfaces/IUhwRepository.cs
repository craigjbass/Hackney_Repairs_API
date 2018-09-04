using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HackneyRepairs.Entities;
using HackneyRepairs.Models;

namespace HackneyRepairs.Interfaces
{
    public interface IUhwRepository
    {
        Task AddOrderDocumentAsync(string documentType, string workOrderReference, int workOrderId, string processComment);
		Task<IEnumerable<Note>> GetNotesByWorkOrderReference(string workOrderReference);
        Task<IEnumerable<DetailedNote>> GetNoteFeed(int noteId, string noteTarget, int size, int? remainingCount);
    }
}
