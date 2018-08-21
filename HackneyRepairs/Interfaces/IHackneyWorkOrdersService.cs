using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HackneyRepairs.Entities;
using HackneyRepairs.Models;

namespace HackneyRepairs.Interfaces
{
    public interface IHackneyWorkOrdersService
    {
		Task<UHWorkOrder> GetWorkOrder(string workOrderReference);
		Task<IEnumerable<UHWorkOrderBase>> GetWorkOrderByPropertyReference(string propertyReference);
		Task<IEnumerable<Note>> GetNotesByWorkOrderReference(string workOrderReference);
    }
}
