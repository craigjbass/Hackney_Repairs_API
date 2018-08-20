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
		public Task<UHWorkOrderExtended> GetWorkOrder(string workOrderReference)
		{
			if (string.Equals(workOrderReference, "9999999999"))
			{
				return Task.Run(() => (UHWorkOrderExtended)null);
			}
			var workOrder = new UHWorkOrderExtended
			{
				WorkOrderReference = workOrderReference
			};
			return Task.Run(() => workOrder);
		}

		public Task<IEnumerable<UHWorkOrder>> GetWorkOrderByPropertyReference(string propertyReference)
        {
            if (string.Equals(propertyReference, "9999999999"))
            {
				return Task.Run(() => (IEnumerable<UHWorkOrder>)new List<UHWorkOrder>());
            }
			var workOrder = new List<UHWorkOrder>
            {
				new UHWorkOrder
                {
                    PropertyReference = propertyReference
                }
            };
			return Task.Run(() => (IEnumerable<UHWorkOrder>)workOrder);
        }

		public Task<IEnumerable<NotesEntity>> GetNotesByWorkOrderReference(string workOrderReference)
        {
			if (string.Equals(workOrderReference, "99999999"))
            {
				return Task.Run(() => (IEnumerable<NotesEntity>)new List<NotesEntity>());
            }
			var noteEntities = new List<NotesEntity>
            {
				new NotesEntity{
					KeyObject = "UHOrder",
					NoteText = "Some note"

				}
          
            };
			return Task.Run(() => (IEnumerable<NotesEntity>)noteEntities);
        }
	}
}
