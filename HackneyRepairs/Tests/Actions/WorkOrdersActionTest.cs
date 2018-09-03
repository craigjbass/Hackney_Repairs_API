using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HackneyRepairs.Actions;
using HackneyRepairs.Entities;
using HackneyRepairs.Interfaces;
using HackneyRepairs.Models;
using Moq;
using Xunit;

namespace HackneyRepairs.Tests.Actions
{
	public class WorkOrdersActionTest
	{
		Mock<ILoggerAdapter<WorkOrdersActions>> _mockLogger;
		public WorkOrdersActionTest()
		{
			_mockLogger = new Mock<ILoggerAdapter<WorkOrdersActions>>();
		}

		#region GetWorkOrder 
		[Fact]
		public async Task get_work_orders_returns_a_work_order()
		{
			Mock<IHackneyWorkOrdersService> _workOrderService = new Mock<IHackneyWorkOrdersService>();
			_workOrderService.Setup(service => service.GetWorkOrder(It.IsAny<string>()))
			                 .ReturnsAsync(new UHWorkOrder());
			WorkOrdersActions workOrdersActions = new WorkOrdersActions(_workOrderService.Object, _mockLogger.Object);

			var response = await workOrdersActions.GetWorkOrder("12345678");

			Assert.True(response is UHWorkOrder);
		}

		[Fact]
		public async Task get_work_orders_returns_a_work_order_with_the_same_ref_as_paramater()
		{
			Random rnd = new Random();
			string randomReference = rnd.Next(10000000, 99999999).ToString();
			Mock<IHackneyWorkOrdersService> _workOrderService = new Mock<IHackneyWorkOrdersService>();
			_workOrderService.Setup(service => service.GetWorkOrder(randomReference))
			                 .Returns(Task.FromResult(new UHWorkOrder { WorkOrderReference = randomReference }));
			WorkOrdersActions workOrdersActions = new WorkOrdersActions(_workOrderService.Object, _mockLogger.Object);

			var response = await workOrdersActions.GetWorkOrder(randomReference);
			var expected = new UHWorkOrder
			{
				WorkOrderReference = randomReference
			};

			Assert.Equal(response.WorkOrderReference, expected.WorkOrderReference);
		}

		[Fact]
		public async Task get_work_orders_throws_not_found_excpetion_when_no_work_order_found()
		{
			Random rnd = new Random();
			string randomReference = rnd.Next(100000000, 999999990).ToString();
			Mock<IHackneyWorkOrdersService> _workOrderService = new Mock<IHackneyWorkOrdersService>();
			_workOrderService.Setup(service => service.GetWorkOrder(randomReference))
			                 .Returns(Task.FromResult((UHWorkOrder)null));
			WorkOrdersActions workOrdersActions = new WorkOrdersActions(_workOrderService.Object, _mockLogger.Object);
			await Assert.ThrowsAsync<MissingWorkOrderException>(async () => await workOrdersActions.GetWorkOrder(randomReference));
		}
		#endregion

		#region GetWorkOrderByPropertyReference
		[Fact]
		public async Task get_by_property_reference_returns_list_work_orders()
		{
			List<UHWorkOrderBase> fakeResponse = new List<UHWorkOrderBase>
			{
				new UHWorkOrderBase()
			};
			Mock<IHackneyWorkOrdersService> _workOrderService = new Mock<IHackneyWorkOrdersService>();
			_workOrderService.Setup(service => service.GetWorkOrderByPropertyReference(It.IsAny<string>()))
			                 .Returns(Task.FromResult<IEnumerable<UHWorkOrderBase>>(fakeResponse));
			WorkOrdersActions workOrdersActions = new WorkOrdersActions(_workOrderService.Object,
																		_mockLogger.Object);
			var response = await workOrdersActions.GetWorkOrdersByPropertyReference("12345678");

			Assert.True(response is List<UHWorkOrderBase>);
		}

		[Fact]
		public async Task get_by_property_reference_returns_work_order_with_the_same_ref_as_parameter()
		{
			Random rnd = new Random();
			string randomReference = rnd.Next(10000000, 99999999).ToString();

			var fakeResponse = new List<UHWorkOrderBase>
			{
				new UHWorkOrderBase { PropertyReference = randomReference }
			};

			Mock<IHackneyWorkOrdersService> _workOrderService = new Mock<IHackneyWorkOrdersService>();
			_workOrderService.Setup(service => service.GetWorkOrderByPropertyReference(randomReference))
			                 .Returns(Task.FromResult<IEnumerable<UHWorkOrderBase>>(fakeResponse));
			WorkOrdersActions workOrdersActions = new WorkOrdersActions(_workOrderService.Object, _mockLogger.Object);

			var response = await workOrdersActions.GetWorkOrderByPropertyReference(randomReference);
			var expected = new List<UHWorkOrderBase>
			{
				new UHWorkOrderBase { PropertyReference = randomReference }
			};
			Assert.Equal(response.FirstOrDefault().PropertyReference, expected.FirstOrDefault().PropertyReference);
		}

		[Fact]
		public async Task get_by_property_reference_throws_not_found_exception_when_no_results()
		{
			Random rnd = new Random();
			string randomReference = rnd.Next(100000000, 999999990).ToString();
			Mock<IHackneyWorkOrdersService> _workOrderService = new Mock<IHackneyWorkOrdersService>();
			_workOrderService.Setup(service => service.GetWorkOrderByPropertyReference(randomReference))
			                 .Returns(Task.FromResult<IEnumerable<UHWorkOrderBase>>((new List<UHWorkOrderBase>())));
			WorkOrdersActions workOrdersActions = new WorkOrdersActions(_workOrderService.Object, _mockLogger.Object);

			await Assert.ThrowsAsync<MissingWorkOrderException>(async () => await workOrdersActions.GetWorkOrdersByPropertyReference(randomReference));
		}
		#endregion

		#region Get notes for work order
		[Fact]
        public async Task get_notes_by_work_order_reference_returns_a_list_of_notes()
        {
			Random rnd = new Random();
            string randomReference = rnd.Next(100000000, 999999999).ToString();
			List<Note> fakeResponse = new List<Note>
            {
				new Note()
            };
            Mock<IHackneyWorkOrdersService> _workOrderService = new Mock<IHackneyWorkOrdersService>();
            _workOrderService.Setup(service => service.GetNotesByWorkOrderReference(It.IsAny<string>()))
			                 .Returns(Task.FromResult<IEnumerable<Note>>(fakeResponse));
            WorkOrdersActions workOrdersActions = new WorkOrdersActions(_workOrderService.Object,
                                                                        _mockLogger.Object);
			var response = await workOrdersActions.GetNotesByWorkOrderReference(randomReference);
            
			Assert.True(response is List<NotesEntity>);
        }

        [Fact]
        public async Task get_notes_by_workorder_reference_throws_not_found_exception_when_no_results()
        {
            Random rnd = new Random();
            string randomReference = rnd.Next(100000000, 999999999).ToString();
            Mock<IHackneyWorkOrdersService> _workOrderService = new Mock<IHackneyWorkOrdersService>();
            _workOrderService.Setup(service => service.GetNotesByWorkOrderReference(randomReference))
			                 .Returns(Task.FromResult<IEnumerable<Note>>((new List<Note>())));
            WorkOrdersActions workOrdersActions = new WorkOrdersActions(_workOrderService.Object, _mockLogger.Object);

			await Assert.ThrowsAsync<MissingNotesException>(async () => await workOrdersActions.GetNotesByWorkOrderReference(randomReference));
        }
        #endregion
	}
}
