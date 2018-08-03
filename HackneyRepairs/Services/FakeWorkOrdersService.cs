using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HackneyRepairs.Entities;
using HackneyRepairs.Interfaces;

namespace HackneyRepairs.Services
{
	public class FakeWorkOrdersService : IHackneyWorkOrdersService
    {      
        public Task<WorkOrderEntity> GetWorkOrder(string workOrderReference)
		{
			if (string.Equals(workOrderReference, "9999999999"))
			{
				return Task.Run(() => (WorkOrderEntity)null);
			}
			var workOrderEntity = new WorkOrderEntity
			{
				wo_ref = workOrderReference
			};
			return Task.Run(() => workOrderEntity);
		}

        public Task<IEnumerable<WorkOrderEntity>> GetWorkOrderByPropertyReference(string propertyReference)
        {
            if (string.Equals(propertyReference, "9999999999"))
            {
                return Task.Run(() => (IEnumerable<WorkOrderEntity>)new List<WorkOrderEntity>());
            }
            var workOrderEntity = new List<WorkOrderEntity>
            {
                new WorkOrderEntity
                {
                    prop_ref = propertyReference
                }
            };
            return Task.Run(() => (IEnumerable<WorkOrderEntity>)workOrderEntity);
        }
	}
}
