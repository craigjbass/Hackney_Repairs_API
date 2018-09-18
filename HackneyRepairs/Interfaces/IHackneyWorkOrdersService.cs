using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HackneyRepairs.Models;

namespace HackneyRepairs.Interfaces
{
    public interface IHackneyWorkOrdersService
    {
		Task<UHWorkOrder> GetWorkOrder(string workOrderReference);
		Task<IEnumerable<UHWorkOrder>> GetWorkOrderByPropertyReference(string propertyReference);
		Task<IEnumerable<UHWorkOrder>> GetWorkOrderByBlockReference(string blockReference, string trade);
		Task<IEnumerable<Note>> GetNotesByWorkOrderReference(string workOrderReference);
        Task<IEnumerable<Note>> GetNoteFeed(int startId, string noteTarget, int size);
        Task<IEnumerable<UHWorkOrderFeed>> GetWorkOrderFeed(string startId, int resultSize);
    }
}
