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
			Mock<IHackneyPropertyService> _propertyService = new Mock<IHackneyPropertyService>();
            Mock<IHackneyWorkOrdersService> _workOrderService = new Mock<IHackneyWorkOrdersService>();
            _workOrderService.Setup(service => service.GetWorkOrder(It.IsAny<string>()))
                             .ReturnsAsync(new UHWorkOrder());
			WorkOrdersActions workOrdersActions = new WorkOrdersActions(_workOrderService.Object, _propertyService.Object, _mockLogger.Object);

            var response = await workOrdersActions.GetWorkOrder("12345678");

            Assert.True(response is UHWorkOrder);
        }

        [Fact]
        public async Task get_work_orders_returns_a_work_order_with_the_same_ref_as_paramater()
        {
            Random rnd = new Random();
            string randomReference = rnd.Next(10000000, 99999999).ToString();
			Mock<IHackneyPropertyService> _propertyService = new Mock<IHackneyPropertyService>();
            Mock<IHackneyWorkOrdersService> _workOrderService = new Mock<IHackneyWorkOrdersService>();
            _workOrderService.Setup(service => service.GetWorkOrder(randomReference))
                             .Returns(Task.FromResult(new UHWorkOrder { WorkOrderReference = randomReference }));
			WorkOrdersActions workOrdersActions = new WorkOrdersActions(_workOrderService.Object, _propertyService.Object, _mockLogger.Object);

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
			Mock<IHackneyPropertyService> _propertyService = new Mock<IHackneyPropertyService>();
            Mock<IHackneyWorkOrdersService> _workOrderService = new Mock<IHackneyWorkOrdersService>();
            _workOrderService.Setup(service => service.GetWorkOrder(randomReference))
                             .Returns(Task.FromResult((UHWorkOrder)null));
			WorkOrdersActions workOrdersActions = new WorkOrdersActions(_workOrderService.Object, _propertyService.Object, _mockLogger.Object);
            await Assert.ThrowsAsync<MissingWorkOrderException>(async () => await workOrdersActions.GetWorkOrder(randomReference));
        }
        #endregion

        #region GetWorkOrderByPropertyReference
        [Fact]
        public async Task get_by_single_property_reference_returns_list_work_orders()
        {
			Random rnd = new Random();
            string randomReference = rnd.Next(100000000, 999999990).ToString();
			List<UHWorkOrder> fakeResponse = new List<UHWorkOrder>
			{
				new UHWorkOrder()
				{
					PropertyReference = randomReference
				},
				new UHWorkOrder()
                {
					PropertyReference = randomReference
                },
				new UHWorkOrder()
                {
					PropertyReference = randomReference
                }
            };
			Mock<IHackneyPropertyService> _propertyService = new Mock<IHackneyPropertyService>();
            Mock<IHackneyWorkOrdersService> _workOrderService = new Mock<IHackneyWorkOrdersService>();
            _workOrderService.Setup(service => service.GetWorkOrderByPropertyReferences(It.IsAny<List<string>>()))
                             .Returns(Task.FromResult<IEnumerable<UHWorkOrder>>(fakeResponse));
			WorkOrdersActions workOrdersActions = new WorkOrdersActions(_workOrderService.Object, _propertyService.Object, _mockLogger.Object);
			var response = await workOrdersActions.GetWorkOrderByPropertyReferences(randomReference.ToString(), false);

            Assert.True(response is List<UHWorkOrder>);
			Assert.Equal(randomReference, response.FirstOrDefault().PropertyReference);
        }
        
		[Fact]
        public async Task get_by_property_reference_include_children_returns_list_work_orders()
        {
			// Setting up fake property service
			Mock<IHackneyPropertyService> _propertyService = new Mock<IHackneyPropertyService>();
   
			Random rnd = new Random();
            string parentPropReference = rnd.Next(100000000, 999999990).ToString();

            PropertyLevelModel parent = new PropertyLevelModel()
            {
                PropertyReference = parentPropReference
            };

            _propertyService.Setup(service => service.GetPropertyLevelInfo(parentPropReference))
                                   .Returns(Task.FromResult(parent));

            string child1_PropReference = rnd.Next(100000000, 999999990).ToString();
            string child2_PropReference = rnd.Next(100000000, 999999990).ToString();
            string child3_PropReference = rnd.Next(100000000, 999999990).ToString();
            List<PropertyLevelModel> childrenProperties = new List<PropertyLevelModel>
            {
                new PropertyLevelModel()
                {
                    PropertyReference = child1_PropReference,
                    MajorReference = parentPropReference
                },
                new PropertyLevelModel()
                {
                    PropertyReference = child2_PropReference,
                    MajorReference = parentPropReference
                },
                new PropertyLevelModel()
                {
                    PropertyReference = child3_PropReference,
                    MajorReference = parentPropReference
                }
            };
          
			_propertyService.Setup(service => service.GetPropertyLevelInfosForParent(parentPropReference))
			                       .Returns(Task.FromResult(childrenProperties));
			
			_propertyService.Setup(service => service.GetPropertyLevelInfosForParent(It.IsNotIn<string>(parentPropReference)))
			                .Returns(Task.FromResult(new List<PropertyLevelModel>()));

            // Setting up fake work order service         
			//List<UHWorkOrder> parent_workorders = new List<UHWorkOrder>
   //         {
   //             new UHWorkOrder()
   //             {
			//		PropertyReference = parentPropReference
   //             },
   //             new UHWorkOrder()
   //             {
			//		PropertyReference = parentPropReference
   //             },
   //             new UHWorkOrder()
   //             {
			//		PropertyReference = parentPropReference
   //             }
   //         };

			//List<UHWorkOrder> child1_workorders = new List<UHWorkOrder>
   //         {
   //             new UHWorkOrder()
   //             {
			//		PropertyReference = child1_PropReference
   //             },
   //             new UHWorkOrder()
   //             {
			//		PropertyReference = child1_PropReference
   //             },
   //             new UHWorkOrder()
   //             {
			//		PropertyReference = child1_PropReference
   //             }
   //         };
			//List<UHWorkOrder> child2_workorders = new List<UHWorkOrder>
   //         {
   //             new UHWorkOrder()
   //             {
			//		PropertyReference = child2_PropReference
   //             },
   //             new UHWorkOrder()
   //             {
			//		PropertyReference = child2_PropReference
   //             },
   //             new UHWorkOrder()
   //             {
			//		PropertyReference = child2_PropReference
   //             }
   //         };
			List<UHWorkOrder> resultList = new List<UHWorkOrder>
            {
				new UHWorkOrder()
                {
                    PropertyReference = parentPropReference
                },
                new UHWorkOrder()
                {
					PropertyReference = child1_PropReference
                },
                new UHWorkOrder()
                {
					PropertyReference = child2_PropReference
                },
                new UHWorkOrder()
                {
					PropertyReference = child3_PropReference
                }
            };
            

			//resultList.InsertRange(0, parent_workorders);
			//resultList.InsertRange(0, child1_workorders);
			//resultList.InsertRange(0, child2_workorders);
			//resultList.InsertRange(0, child3_workorders);

			//List<string> propRefs = new List<string>();

			//propRefs.Insert(0, parentPropReference);
			//propRefs.Insert(0, child1_PropReference);
			//propRefs.Insert(0, child2_PropReference);
			//propRefs.Insert(0, child3_PropReference);

            Mock<IHackneyWorkOrdersService> _workOrderService = new Mock<IHackneyWorkOrdersService>();
            
			_workOrderService.Setup(service => service.GetWorkOrderByPropertyReferences(It.IsAny<List<string>>()))
			                        .Returns(Task.FromResult<IEnumerable<UHWorkOrder>>(resultList));

            WorkOrdersActions workOrdersActions = new WorkOrdersActions(_workOrderService.Object, _propertyService.Object, _mockLogger.Object);
			var response = await workOrdersActions.GetWorkOrderByPropertyReferences(parentPropReference, true);

            Assert.True(response is List<UHWorkOrder>);
			Assert.True(resultList.Count() == response.Count());
			Assert.True(!resultList.Except(response).Any());
		}


   

        [Fact]
        public async Task get_by_property_reference_throws_not_found_exception_when_no_results()
        {
			// Setting up fake property service
            Mock<IHackneyPropertyService> _propertyService = new Mock<IHackneyPropertyService>();

            Random rnd = new Random();
            string parentPropReference = rnd.Next(100000000, 999999990).ToString();

            PropertyLevelModel parent = new PropertyLevelModel()
            {
                PropertyReference = parentPropReference
            };

            _propertyService.Setup(service => service.GetPropertyLevelInfo(parentPropReference))
                                   .Returns(Task.FromResult(parent));

			string child1_PropReference = rnd.Next(100000000, 999999990).ToString();
            string child2_PropReference = rnd.Next(100000000, 999999990).ToString();
            string child3_PropReference = rnd.Next(100000000, 999999990).ToString();
            List<PropertyLevelModel> childrenProperties = new List<PropertyLevelModel>
            {
                new PropertyLevelModel()
                {
                    PropertyReference = child1_PropReference,
                    MajorReference = parentPropReference
                },
                new PropertyLevelModel()
                {
                    PropertyReference = child2_PropReference,
                    MajorReference = parentPropReference
                },
                new PropertyLevelModel()
                {
                    PropertyReference = child3_PropReference,
                    MajorReference = parentPropReference
                }
            };

            _propertyService.Setup(service => service.GetPropertyLevelInfosForParent(parentPropReference))
                                   .Returns(Task.FromResult(childrenProperties));

            _propertyService.Setup(service => service.GetPropertyLevelInfosForParent(It.IsNotIn<string>(parentPropReference)))
                            .Returns(Task.FromResult(new List<PropertyLevelModel>()));


            
            Mock<IHackneyWorkOrdersService> _workOrderService = new Mock<IHackneyWorkOrdersService>();
			_workOrderService.Setup(service => service.GetWorkOrderByPropertyReferences(It.IsAny<List<string>>()))
                             .Returns(Task.FromResult<IEnumerable<UHWorkOrder>>((new List<UHWorkOrder>())));

			WorkOrdersActions workOrdersActions = new WorkOrdersActions(_workOrderService.Object, _propertyService.Object, _mockLogger.Object);

			await Assert.ThrowsAsync<MissingWorkOrderException>(async () => await workOrdersActions.GetWorkOrderByPropertyReferences(parentPropReference, true));
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
			Mock<IHackneyPropertyService> _propertyService = new Mock<IHackneyPropertyService>();
            Mock<IHackneyWorkOrdersService> _workOrderService = new Mock<IHackneyWorkOrdersService>();
            _workOrderService.Setup(service => service.GetNotesByWorkOrderReference(It.IsAny<string>()))
                             .Returns(Task.FromResult<IEnumerable<Note>>(fakeResponse));
			WorkOrdersActions workOrdersActions = new WorkOrdersActions(_workOrderService.Object, _propertyService.Object, _mockLogger.Object);
            var response = await workOrdersActions.GetNotesByWorkOrderReference(randomReference);

            Assert.True(response is List<Note>);
        }

        [Fact]
        public async Task get_notes_by_workorder_reference_throws_not_found_exception_when_no_results()
        {
            Random rnd = new Random();
            string randomReference = rnd.Next(100000000, 999999999).ToString();
			Mock<IHackneyPropertyService> _propertyService = new Mock<IHackneyPropertyService>(); 
            Mock<IHackneyWorkOrdersService> _workOrderService = new Mock<IHackneyWorkOrdersService>();
            _workOrderService.Setup(service => service.GetNotesByWorkOrderReference(randomReference))
                             .Returns(Task.FromResult<IEnumerable<Note>>((new List<Note>())));
			WorkOrdersActions workOrdersActions = new WorkOrdersActions(_workOrderService.Object, _propertyService.Object, _mockLogger.Object);
            await Assert.ThrowsAsync<MissingNotesException>(async () => await workOrdersActions.GetNotesByWorkOrderReference(randomReference));
        }
        #endregion

        #region Get workOrder feed
        [Fact]
        public async Task get_workorder_feed_returns_a_list_of_uhworkorderfeed()
        {
            Random rnd = new Random();
            string randomReference = rnd.Next(100000000, 999999999).ToString();
            int randomSize = rnd.Next(1, 50);
			Mock<IHackneyPropertyService> _propertyService = new Mock<IHackneyPropertyService>();
            Mock<IHackneyWorkOrdersService> _workOrderService = new Mock<IHackneyWorkOrdersService>();
            _workOrderService.Setup(service => service.GetWorkOrderFeed(It.IsAny<string>(), It.IsAny<int>()))
                             .Returns(Task.FromResult<IEnumerable<UHWorkOrderFeed>>(new List<UHWorkOrderFeed>()));
			WorkOrdersActions workOrdersActions = new WorkOrdersActions(_workOrderService.Object, _propertyService.Object, _mockLogger.Object);
            var response = await workOrdersActions.GetWorkOrdersFeed(randomReference, randomSize);

            Assert.True(response is List<UHWorkOrderFeed>);
        }
        #endregion
    }
}
