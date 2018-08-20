using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HackneyRepairs.Entities;
using HackneyRepairs.Models;

namespace HackneyRepairs.Interfaces
{
    public interface IHackneyWorkOrdersService
    {
		Task<UHWorkOrderExtended> GetWorkOrder(string workOrderReference);
		Task<IEnumerable<UHWorkOrder>> GetWorkOrderByPropertyReference(string propertyReference);
		Task<IEnumerable<NotesEntity>> GetNotesByWorkOrderReference(string workOrderReference);
    }
}
