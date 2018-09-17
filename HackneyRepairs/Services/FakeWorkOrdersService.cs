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
        
		public Task<IEnumerable<UHWorkOrder>> GetWorkOrderByPropertyReferences(IEnumerable<string> propertyReference)
        {
            if (string.Equals(propertyReference, "9999999999"))
            {
				return Task.Run(() => (IEnumerable<UHWorkOrder>)new List<UHWorkOrder>());
            }
			var workOrder = new List<UHWorkOrder>
            {
				new UHWorkOrder
                {
					PropertyReference = propertyReference.First()
                }
            };
			return Task.Run(() => (IEnumerable<UHWorkOrder>)workOrder);
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

        public Task<IEnumerable<Note>> GetNoteFeed(int startId, string noteTarget, int resultSize)
        {
            if (startId == 99999999)
            {
                return Task.Run(() => (IEnumerable<Note>)new List<Note>());
            }
            if (startId == 11550853)
            {
                throw new FakeWorkOrdersServiceException();
            }
            var fakeNoteResponse = new List<Note>
            {
                new Note()
                {
                    WorkOrderReference = "123"
                }
            };
            return Task.Run(() => (IEnumerable<Note>)fakeNoteResponse);
        }
    }

    class FakeWorkOrdersServiceException : Exception {}
}
