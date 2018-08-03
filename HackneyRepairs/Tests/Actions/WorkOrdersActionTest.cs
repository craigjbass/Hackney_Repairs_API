using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HackneyRepairs.Actions;
using HackneyRepairs.Entities;
using HackneyRepairs.Interfaces;
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
                             .ReturnsAsync(new WorkOrderEntity());
            WorkOrdersActions workOrdersActions = new WorkOrdersActions(_workOrderService.Object, _mockLogger.Object);

            var response = await workOrdersActions.GetWorkOrder("12345678");

            Assert.True(response is WorkOrderEntity);
        }

        [Fact]
        public async Task get_work_orders_returns_a_work_order_with_the_same_ref_as_paramater()
        {
            Random rnd = new Random();
            string randomReference = rnd.Next(10000000, 99999999).ToString();
            Mock<IHackneyWorkOrdersService> _workOrderService = new Mock<IHackneyWorkOrdersService>();
            _workOrderService.Setup(service => service.GetWorkOrder(randomReference))
                             .Returns(Task.FromResult(new WorkOrderEntity { wo_ref = randomReference }));
            WorkOrdersActions workOrdersActions = new WorkOrdersActions(_workOrderService.Object, _mockLogger.Object);

            var response = await workOrdersActions.GetWorkOrder(randomReference);
            var expected = new WorkOrderEntity
            {
                wo_ref = randomReference
            };

            Assert.Equal(response.wo_ref, expected.wo_ref);
        }

        [Fact]
        public async Task get_work_orders_throws_not_found_excpetion_when_no_work_order_found()
        {
            Random rnd = new Random();
            string randomReference = rnd.Next(100000000, 999999990).ToString();
            Mock<IHackneyWorkOrdersService> _workOrderService = new Mock<IHackneyWorkOrdersService>();
            _workOrderService.Setup(service => service.GetWorkOrder(randomReference))
                             .Returns(Task.FromResult((WorkOrderEntity)null));
            WorkOrdersActions workOrdersActions = new WorkOrdersActions(_workOrderService.Object, _mockLogger.Object);
            await Assert.ThrowsAsync<MissingWorkOrderException>(async () => await workOrdersActions.GetWorkOrder(randomReference));
        }
        #endregion

        #region GetWorkOrderByPropertyReference
        [Fact]
        public async Task get_by_property_reference_returns_list_work_orders()
        {
            List<WorkOrderEntity> fakeResponse = new List<WorkOrderEntity>
            {
                new WorkOrderEntity()
            };
            Mock<IHackneyWorkOrdersService> _workOrderService = new Mock<IHackneyWorkOrdersService>();
            _workOrderService.Setup(service => service.GetWorkOrderByPropertyReference(It.IsAny<string>()))
                             .Returns(Task.FromResult<IEnumerable<WorkOrderEntity>>(fakeResponse));
            WorkOrdersActions workOrdersActions = new WorkOrdersActions(_workOrderService.Object,
                                                                        _mockLogger.Object);
            var response = await workOrdersActions.GetWorkOrderByPropertyReference("12345678");

            Assert.True(response is List<WorkOrderEntity>);
        }

        [Fact]
        public async Task get_by_property_reference_returns_work_order_with_the_same_ref_as_parameter()
        {
            Random rnd = new Random();
            string randomReference = rnd.Next(10000000, 99999999).ToString();

            var fakeResponse = new List<WorkOrderEntity>
            {
                new WorkOrderEntity { prop_ref = randomReference }
            };
        
            Mock<IHackneyWorkOrdersService> _workOrderService = new Mock<IHackneyWorkOrdersService>();
            _workOrderService.Setup(service => service.GetWorkOrderByPropertyReference(randomReference))
                             .Returns(Task.FromResult<IEnumerable<WorkOrderEntity>>(fakeResponse));
            WorkOrdersActions workOrdersActions = new WorkOrdersActions(_workOrderService.Object, _mockLogger.Object);

            var response = await workOrdersActions.GetWorkOrderByPropertyReference(randomReference);
            var expected = new List<WorkOrderEntity>
            {
                new WorkOrderEntity { prop_ref = randomReference }
            };

            Assert.Equal(response.FirstOrDefault().prop_ref, expected.FirstOrDefault().prop_ref);
        }

        [Fact]
        public async Task get_by_property_reference_throws_not_found_exception_when_no_results()
        {
            Random rnd = new Random();
            string randomReference = rnd.Next(100000000, 999999990).ToString();
            Mock<IHackneyWorkOrdersService> _workOrderService = new Mock<IHackneyWorkOrdersService>();
            _workOrderService.Setup(service => service.GetWorkOrderByPropertyReference(randomReference))
                             .Returns(Task.FromResult<IEnumerable<WorkOrderEntity>>((new List<WorkOrderEntity>())));
            WorkOrdersActions workOrdersActions = new WorkOrdersActions(_workOrderService.Object, _mockLogger.Object);

            await Assert.ThrowsAsync<MissingWorkOrderException>(async () => await workOrdersActions.GetWorkOrderByPropertyReference(randomReference)); 
        }
        # endregion
    }
}
