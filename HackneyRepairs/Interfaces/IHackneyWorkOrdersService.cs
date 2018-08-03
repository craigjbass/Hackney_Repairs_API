using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HackneyRepairs.Entities;

namespace HackneyRepairs.Interfaces
{
    public interface IHackneyWorkOrdersService
    {
		Task<WorkOrderEntity> GetWorkOrder(string workOrderReference);
        Task<IEnumerable<WorkOrderEntity>> GetWorkOrderByPropertyReference(string propertyReference);
    }
}
