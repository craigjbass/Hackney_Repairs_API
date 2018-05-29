using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HackneyRepairs.Actions;
using HackneyRepairs.Services;
using HackneyRepairs.DbContext;
using HackneyRepairs.Interfaces;
using HackneyRepairs.Models;
using HackneyRepairs.Repository;
using Moq;
using Xunit;

namespace HackneyRepairs.Tests.Services
{
    public class HackneyRepairServiceTests
    {
        [Fact]
        public async Task Get_workorder_details_by_reference_returns_object()
        {
            var mockLogger = new Mock<ILoggerAdapter<RepairsActions>>();
            var drsOrder = new DrsOrder
            {
               postcode = "E8 2LN",
               prop_ref = "12345",
               priority = "N"
            };
            var mockRepository = new Mock<IUhtRepository>();
            var mockUHWRepository = new Mock<IUhwRepository>();
            mockRepository.Setup(repo => repo.GetWorkOrderDetails("123456")).ReturnsAsync(drsOrder);
            var repairsService = new HackneyRepairsService(mockRepository.Object, mockUHWRepository.Object, mockLogger.Object);
            var workOrder = repairsService.GetWorkOrderDetails("123456").Result;
            Assert.Equal(workOrder.postcode, drsOrder.postcode);
            Assert.Equal(workOrder.prop_ref, drsOrder.prop_ref);
            Assert.Equal(workOrder.priority, drsOrder.priority);
        }
    }
}
