using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HackneyRepairs.Entities;
using HackneyRepairs.Interfaces;
using HackneyRepairs.Models;

namespace HackneyRepairs.Services
{
	public class FakeWorkOrdersService : IHackneyWorkOrdersService
    {      
		public Task<UHWorkOrder> GetWorkOrder(string workOrderReference)
		{
			if (string.Equals(workOrderReference, "9999999999"))
			{
				return Task.Run(() => (UHWorkOrder)null);
			}
			var workOrder = new UHWorkOrder
			{
				WorkOrderReference = workOrderReference
			};
			return Task.Run(() => workOrder);
		}

		public Task<IEnumerable<UHWorkOrderBase>> GetWorkOrderByPropertyReference(string propertyReference)
        {
            if (string.Equals(propertyReference, "9999999999"))
            {
				return Task.Run(() => (IEnumerable<UHWorkOrderBase>)new List<UHWorkOrderBase>());
            }
			var workOrder = new List<UHWorkOrderBase>
            {
				new UHWorkOrderBase
                {
                    PropertyReference = propertyReference
                }
            };
			return Task.Run(() => (IEnumerable<UHWorkOrderBase>)workOrder);
        }

		public Task<IEnumerable<Note>> GetNotesByWorkOrderReference(string workOrderReference)
        {
			if (string.Equals(workOrderReference, "99999999"))
            {
				return Task.Run(() => (IEnumerable<Note>)new List<Note>());
            }
            var noteEntities = new List<Note>
            {
                new Note()
                {
                    Text = "Some note",
                    LoggedBy = "UHOrder"
				}
          
            };
			return Task.Run(() => (IEnumerable<Note>)noteEntities);
        }
	}
}
