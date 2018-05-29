using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DrsAppointmentsService;
using HackneyRepairs.Actions;
using HackneyRepairs.Interfaces;
using HackneyRepairs.Models;
using Moq;
using Xunit;

namespace HackneyRepairs.Tests.Actions
{
    public class AppointmentActionsTests
    {
        [Fact]
        public async Task get_appointments_returns_a_list_of_available_appointments()
        {
            var xmbResponse = new xmbCheckAvailabilityResponse
            {
                status = responseStatus.success,
                theSlots = new List<daySlotsInfo>
                {
                    new daySlotsInfo
                    {
                        day = new DateTime(2017, 10, 18, 00, 00, 00),
                        nonWorkingDay = false,
                        daySpecified = false,
                        weeklyDayOff = false,
                        slotsForDay = new List<slotInfo>
                        {
                            new slotInfo
                            {
                                available = availableValue.YES,
                                beginDate = new DateTime(2017, 10, 18, 10, 00, 00),
                                endDate = new DateTime(2017, 10, 18, 12, 00, 00),
                                bestSlot = true
                            },
                            new slotInfo
                            {
                                available = availableValue.NO,
                                beginDate = new DateTime(2017, 10, 18, 12, 00, 00),
                                endDate = new DateTime(2017, 10, 18, 14, 00, 00),
                                bestSlot = false
                            },
                            new slotInfo
                            {
                                available = availableValue.MAYBE,
                                beginDate = new DateTime(2017, 10, 18, 14, 00, 00),
                                endDate = new DateTime(2017, 10, 18, 16, 00, 00),
                                bestSlot = false
                            }
                        }.ToArray()
                    }
                }.ToArray()
            };
            var response = new checkAvailabilityResponse(xmbResponse);
            var mockLogger = new Mock<ILoggerAdapter<AppointmentActions>>();
            var mockAppointmentsService = new Mock<IHackneyAppointmentsService>();
            mockAppointmentsService.Setup(service => service.GetAppointmentsForWorkOrderReference(It.IsAny<xmbCheckAvailability>()))
                .ReturnsAsync(response);
            var openSessionResponse = new openSessionResponse(
                new xmbOpenSessionResponse
                {
                    status = responseStatus.success,
                    sessionId = "123456"
                }
            );
            var closeSessionResponse = new closeSessionResponse(
                new xmbCloseSessionResponse
                {
                    status = responseStatus.success
                }
            );
            var drsOrder = new DrsOrder { wo_ref = "01550854" };
            var xmbCheckAvailabilty = new xmbCheckAvailability { theOrder = new order { primaryOrderNumber = "01550854" } };
            var xmbCreateOrder =
                new xmbCreateOrder { sessionId = "123456", theOrder = new order { primaryOrderNumber = "01550864" } };
            var createOrderResponse = new createOrderResponse
            {
                @return = new xmbCreateOrderResponse
                {
                    status = responseStatus.success,
                    theOrder = new order { orderId = 123 }
                }
            };
            mockAppointmentsService.Setup(service => service.OpenSessionAsync(It.IsAny<xmbOpenSession>()))
                .ReturnsAsync(openSessionResponse);
            mockAppointmentsService.Setup(service => service.CloseSessionAsync(It.IsAny<xmbCloseSession>()))
                .ReturnsAsync(closeSessionResponse);
            mockAppointmentsService.Setup(service => service.CreateWorkOrderAsync(xmbCreateOrder))
                .ReturnsAsync(createOrderResponse);
            var mockRepairsService  =new Mock<IHackneyRepairsService>();
            mockRepairsService.Setup(service => service.GetWorkOrderDetails("01550854")).ReturnsAsync(drsOrder);
            var fakeRequestBuilder = new Mock<IHackneyAppointmentsServiceRequestBuilder>();
            fakeRequestBuilder.Setup(service => service.BuildXmbCheckAvailabilityRequest("01550854", "123456", drsOrder, DateTime.Now, DateTime.Now.AddDays(7))).Returns(xmbCheckAvailabilty);
            fakeRequestBuilder.Setup(service => service.BuildXmbOpenSessionRequest()).Returns(new xmbOpenSession());
            fakeRequestBuilder.Setup(service => service.BuildXmbCreateOrderRequest("01550854", "123456", drsOrder))
                .Returns(xmbCreateOrder);
            var fakeRepairRequestBuilder = new Mock<IHackneyRepairsServiceRequestBuilder>();
            AppointmentActions appointmentActions = new AppointmentActions(mockLogger.Object, mockAppointmentsService.Object, fakeRequestBuilder.Object, mockRepairsService.Object, fakeRepairRequestBuilder.Object);
            var results = await appointmentActions.GetAppointments("01550854");
            Assert.Equal(1, results.Count);
        }

        [Fact]
        public async Task get_appointments_raises_an_exception_when_drs_service_responds_with_error_when_opening_session()
        {
            var xmbResponse = new xmbCheckAvailabilityResponse
            {
                status = responseStatus.success,
                theSlots = new List<daySlotsInfo>().ToArray()
            };
            var response = new checkAvailabilityResponse(xmbResponse);
            var mockLogger = new Mock<ILoggerAdapter<AppointmentActions>>();
            var mockAppointmentsService = new Mock<IHackneyAppointmentsService>();
            mockAppointmentsService.Setup(service => service.GetAppointmentsForWorkOrderReference(It.IsAny<xmbCheckAvailability>()))
                .ReturnsAsync(response);
            var sessionResponse = new openSessionResponse(
                new xmbOpenSessionResponse
                {
                    status = responseStatus.error,
                    sessionId = ""
                }
            );
            var drsOrder = new DrsOrder();
            mockAppointmentsService.Setup(service => service.OpenSessionAsync(It.IsAny<xmbOpenSession>()))
                .ReturnsAsync(sessionResponse);
            var mockRepairsService = new Mock<IHackneyRepairsService>();
            mockRepairsService.Setup(service => service.GetWorkOrderDetails("0000123")).ReturnsAsync(drsOrder);
            var fakeRequestBuilder = new Mock<IHackneyAppointmentsServiceRequestBuilder>();
            fakeRequestBuilder.Setup(service => service.BuildXmbCheckAvailabilityRequest("0000123", "", drsOrder, DateTime.Now, DateTime.Now.AddDays(7))).Returns(new xmbCheckAvailability());
            fakeRequestBuilder.Setup(service => service.BuildXmbOpenSessionRequest()).Returns(new xmbOpenSession());
            var fakeRepairRequestBuilder = new Mock<IHackneyRepairsServiceRequestBuilder>();
            AppointmentActions appointmentActions = new AppointmentActions(mockLogger.Object, mockAppointmentsService.Object, fakeRequestBuilder.Object, mockRepairsService.Object, fakeRepairRequestBuilder.Object);
            await Assert.ThrowsAsync<AppointmentServiceException>(async () => await appointmentActions.GetAppointments("01550854"));
        }

        [Fact]
        public async Task get_appointments_raises_an_exception_when_drs_service_responds_with_error_when_closing_session()
        {
            var xmbResponse = new xmbCheckAvailabilityResponse
            {
                status = responseStatus.success,
                theSlots = new List<daySlotsInfo>().ToArray()
            };
            var response = new checkAvailabilityResponse(xmbResponse);
            var mockLogger = new Mock<ILoggerAdapter<AppointmentActions>>();
            var mockAppointmentsService = new Mock<IHackneyAppointmentsService>();
            mockAppointmentsService.Setup(service => service.GetAppointmentsForWorkOrderReference(It.IsAny<xmbCheckAvailability>()))
                .ReturnsAsync(response);
            var openSessionResponse = new openSessionResponse(
                new xmbOpenSessionResponse
                {
                    status = responseStatus.error,
                    sessionId = ""
                }
            );
            var closeSessionResponse = new closeSessionResponse(
                new xmbCloseSessionResponse
                {
                    status = responseStatus.error
                }
            );
            var drsOrder = new DrsOrder();
            mockAppointmentsService.Setup(service => service.OpenSessionAsync(It.IsAny<xmbOpenSession>()))
                .ReturnsAsync(openSessionResponse);
            mockAppointmentsService.Setup(service => service.CloseSessionAsync(It.IsAny<xmbCloseSession>()))
                .ReturnsAsync(closeSessionResponse);
            var mockRepairsService = new Mock<IHackneyRepairsService>();
            mockRepairsService.Setup(service => service.GetWorkOrderDetails("0000123")).ReturnsAsync(drsOrder);
            var fakeRequestBuilder = new Mock<IHackneyAppointmentsServiceRequestBuilder>();
            fakeRequestBuilder.Setup(service => service.BuildXmbCheckAvailabilityRequest("0000123", "", drsOrder, DateTime.Now, DateTime.Now.AddDays(7))).Returns(new xmbCheckAvailability());
            fakeRequestBuilder.Setup(service => service.BuildXmbOpenSessionRequest()).Returns(new xmbOpenSession());
            var fakeRepairRequestBuilder = new Mock<IHackneyRepairsServiceRequestBuilder>();
            AppointmentActions appointmentActions = new AppointmentActions(mockLogger.Object, mockAppointmentsService.Object, fakeRequestBuilder.Object, mockRepairsService.Object, fakeRepairRequestBuilder.Object);
            await Assert.ThrowsAsync<AppointmentServiceException>(async () => await appointmentActions.GetAppointments("01550854"));
        }

        [Fact]
        public async Task get_appointments_raises_an_exception_if_the_slots_list_is_missing()
        {
            var xmbResponse = new xmbCheckAvailabilityResponse
            {
                status = responseStatus.success,
                theSlots = null
            };
            var response = new checkAvailabilityResponse(xmbResponse);
            var mockLogger = new Mock<ILoggerAdapter<AppointmentActions>>();
            var mockAppointmentsService = new Mock<IHackneyAppointmentsService>();
            mockAppointmentsService.Setup(service => service.GetAppointmentsForWorkOrderReference(It.IsAny<xmbCheckAvailability>()))
                .ReturnsAsync(response);
            var sessionResponse = new openSessionResponse(
                new xmbOpenSessionResponse
                {
                    status = responseStatus.success,
                    sessionId = "123456"
                }
            );
            var closeSessionResponse = new closeSessionResponse(
                new xmbCloseSessionResponse
                {
                    status = responseStatus.success
                }
            );
            var drsOrder = new DrsOrder { wo_ref = "01550854" };
            var xmbCheckAvailabilty = new xmbCheckAvailability { theOrder = new order { primaryOrderNumber = "01550854" } };
            var xmbCreateOrder =
                new xmbCreateOrder { sessionId = "123456", theOrder = new order { primaryOrderNumber = "01550864" } };
            var createOrderResponse = new createOrderResponse
            {
                @return = new xmbCreateOrderResponse
                {
                    status = responseStatus.success,
                    theOrder = new order { orderId = 123 }
                }
            };
            mockAppointmentsService.Setup(service => service.OpenSessionAsync(It.IsAny<xmbOpenSession>()))
                .ReturnsAsync(sessionResponse);
            mockAppointmentsService.Setup(service => service.CloseSessionAsync(It.IsAny<xmbCloseSession>()))
                .ReturnsAsync(closeSessionResponse);
            mockAppointmentsService.Setup(service => service.CreateWorkOrderAsync(xmbCreateOrder))
                .ReturnsAsync(createOrderResponse);
            var mockRepairsService = new Mock<IHackneyRepairsService>();
            mockRepairsService.Setup(service => service.GetWorkOrderDetails("01550854")).ReturnsAsync(drsOrder);
            var fakeRequestBuilder = new Mock<IHackneyAppointmentsServiceRequestBuilder>();
            fakeRequestBuilder.Setup(service => service.BuildXmbCheckAvailabilityRequest("01550854", "123456", drsOrder, DateTime.Now, DateTime.Now.AddDays(7))).Returns(xmbCheckAvailabilty);
            fakeRequestBuilder.Setup(service => service.BuildXmbCreateOrderRequest("01550854", "123456", drsOrder))
                .Returns(xmbCreateOrder);
            var fakeRepairRequestBuilder = new Mock<IHackneyRepairsServiceRequestBuilder>();
            AppointmentActions appointmentActions = new AppointmentActions(mockLogger.Object, mockAppointmentsService.Object, fakeRequestBuilder.Object, mockRepairsService.Object, fakeRepairRequestBuilder.Object);
            await Assert.ThrowsAsync<MissingSlotsException>(async () => await appointmentActions.GetAppointments("01550854"));
        }

        [Fact]
        public async Task get_appointments_raises_an_exception_if_the_service_responds_with_an_error()
        {
            var xmbResponse = new xmbCheckAvailabilityResponse
            {
                status = responseStatus.error,
                theSlots = new List<daySlotsInfo>().ToArray()
            };
            var response = new checkAvailabilityResponse(xmbResponse);
            var mockLogger = new Mock<ILoggerAdapter<AppointmentActions>>();
            var mockAppointmentsService = new Mock<IHackneyAppointmentsService>();
            mockAppointmentsService.Setup(service => service.GetAppointmentsForWorkOrderReference(It.IsAny<xmbCheckAvailability>()))
                .ReturnsAsync(response);
            var sessionResponse = new openSessionResponse(
                new xmbOpenSessionResponse
                {
                    status = responseStatus.success,
                    sessionId = "123456"
                }
            );
            var closeSessionResponse = new closeSessionResponse(
                new xmbCloseSessionResponse
                {
                    status = responseStatus.error
                }
            );
            var drsOrder = new DrsOrder{wo_ref = "01550854"};
            var xmbCheckAvailabilty = new xmbCheckAvailability { theOrder = new order { primaryOrderNumber = "01550854" } };
            var xmbCreateOrder =
                new xmbCreateOrder { sessionId = "123456", theOrder = new order { primaryOrderNumber = "01550864" } };
            var createOrderResponse = new createOrderResponse
            {
                @return = new xmbCreateOrderResponse
                {
                    status = responseStatus.success,
                    theOrder = new order { orderId = 123 }
                }
            };
            mockAppointmentsService.Setup(service => service.OpenSessionAsync(It.IsAny<xmbOpenSession>()))
                .ReturnsAsync(sessionResponse);
            mockAppointmentsService.Setup(service => service.CloseSessionAsync(It.IsAny<xmbCloseSession>()))
                .ReturnsAsync(closeSessionResponse);
            mockAppointmentsService.Setup(service => service.CreateWorkOrderAsync(xmbCreateOrder))
                .ReturnsAsync(createOrderResponse);
            var mockRepairsService = new Mock<IHackneyRepairsService>();
            mockRepairsService.Setup(service => service.GetWorkOrderDetails("01550854")).ReturnsAsync(drsOrder);
            var fakeRequestBuilder = new Mock<IHackneyAppointmentsServiceRequestBuilder>();
            fakeRequestBuilder.Setup(service => service.BuildXmbCheckAvailabilityRequest("01550854", "123456", drsOrder, DateTime.Now, DateTime.Now.AddDays(7))).Returns(xmbCheckAvailabilty);
            fakeRequestBuilder.Setup(service => service.BuildXmbCreateOrderRequest("01550854", "123456", drsOrder))
                .Returns(xmbCreateOrder);
            var fakeRepairRequestBuilder = new Mock<IHackneyRepairsServiceRequestBuilder>();
            AppointmentActions appointmentActions = new AppointmentActions(mockLogger.Object, mockAppointmentsService.Object, fakeRequestBuilder.Object, mockRepairsService.Object, fakeRepairRequestBuilder.Object);

            await Assert.ThrowsAsync<AppointmentServiceException>(async () => await appointmentActions.GetAppointments("01550854"));
        }

        [Fact]
        public async Task get_appointments_raises_exceptions_if_the_work_order_does_not_exists_in_UH()
        {
            var xmbResponse = new xmbCheckAvailabilityResponse
            {
                status = responseStatus.success,
                theSlots = new List<daySlotsInfo>().ToArray()
            };
            var response = new checkAvailabilityResponse(xmbResponse);
            var mockLogger = new Mock<ILoggerAdapter<AppointmentActions>>();
            var mockAppointmentsService = new Mock<IHackneyAppointmentsService>();
            mockAppointmentsService.Setup(service =>
                    service.GetAppointmentsForWorkOrderReference(It.IsAny<xmbCheckAvailability>()))
                .ReturnsAsync(response);
            var openSessionResponse = new openSessionResponse(
                new xmbOpenSessionResponse
                {
                    status = responseStatus.success,
                    sessionId = "123456"
                }
            );
            var closeSessionResponse = new closeSessionResponse(
                new xmbCloseSessionResponse
                {
                    status = responseStatus.success
                }
            );
            var drsOrder = new DrsOrder();
            mockAppointmentsService.Setup(service => service.OpenSessionAsync(It.IsAny<xmbOpenSession>()))
                .ReturnsAsync(openSessionResponse);
            mockAppointmentsService.Setup(service => service.CloseSessionAsync(It.IsAny<xmbCloseSession>()))
                .ReturnsAsync(closeSessionResponse);
            var mockRepairsService = new Mock<IHackneyRepairsService>();
            mockRepairsService.Setup(service => service.GetWorkOrderDetails("01550854")).ReturnsAsync(drsOrder);
            var fakeRequestBuilder = new Mock<IHackneyAppointmentsServiceRequestBuilder>();
            fakeRequestBuilder
                .Setup(service => service.BuildXmbCheckAvailabilityRequest("01550854", "123456", drsOrder, DateTime.Now, DateTime.Now.AddDays(7)))
                .Returns(new xmbCheckAvailability());
            fakeRequestBuilder.Setup(service => service.BuildXmbOpenSessionRequest()).Returns(new xmbOpenSession());
            fakeRequestBuilder.Setup(service => service.BuildXmbCreateOrderRequest("01550854", "123456", drsOrder))
                .Returns(new xmbCreateOrder());
            var fakeRepairRequestBuilder = new Mock<IHackneyRepairsServiceRequestBuilder>();
            AppointmentActions appointmentActions = new AppointmentActions(mockLogger.Object,
                mockAppointmentsService.Object, fakeRequestBuilder.Object, mockRepairsService.Object, fakeRepairRequestBuilder.Object);
            await Assert.ThrowsAsync<InvalidWorkOrderInUHException>(async () =>
                await appointmentActions.GetAppointments("01550854"));
        }

        [Fact]
        public async Task get_appointments_raises_exceptions_if_service_fails_to_create_order_in_DRS()
        {
            var xmbResponse = new xmbCheckAvailabilityResponse
            {
                status = responseStatus.success,
                theSlots = new List<daySlotsInfo>().ToArray()
            };
            var response = new checkAvailabilityResponse(xmbResponse);
            var mockLogger = new Mock<ILoggerAdapter<AppointmentActions>>();
            var mockAppointmentsService = new Mock<IHackneyAppointmentsService>();
            mockAppointmentsService.Setup(service =>
                    service.GetAppointmentsForWorkOrderReference(It.IsAny<xmbCheckAvailability>()))
                .ReturnsAsync(response);
            var openSessionResponse = new openSessionResponse(
                new xmbOpenSessionResponse
                {
                    status = responseStatus.success,
                    sessionId = "123456"
                }
            );
            var closeSessionResponse = new closeSessionResponse(
                new xmbCloseSessionResponse
                {
                    status = responseStatus.success
                }
            );
            var createOrderResponse = new createOrderResponse
            {
                @return = new xmbCreateOrderResponse
                {
                    status = responseStatus.error,
                    errorMsg = "error creating the order"
                }
            };
            var drsOrder = new DrsOrder{wo_ref = "01550854"};
            var xmbCheckAvailabilty = new xmbCheckAvailability{theOrder = new order{primaryOrderNumber = "01550854"}};
            var xmbCreateOrder =
                new xmbCreateOrder {sessionId = "123456", theOrder = new order {primaryOrderNumber = "01550864"}};
            mockAppointmentsService.Setup(service => service.OpenSessionAsync(It.IsAny<xmbOpenSession>()))
                .ReturnsAsync(openSessionResponse);
            mockAppointmentsService.Setup(service => service.CloseSessionAsync(It.IsAny<xmbCloseSession>()))
                .ReturnsAsync(closeSessionResponse);
            mockAppointmentsService.Setup(service => service.CreateWorkOrderAsync(xmbCreateOrder))
                .ReturnsAsync(createOrderResponse);
            var mockRepairsService = new Mock<IHackneyRepairsService>();
            mockRepairsService.Setup(service => service.GetWorkOrderDetails("01550854")).ReturnsAsync(drsOrder);
            var fakeRequestBuilder = new Mock<IHackneyAppointmentsServiceRequestBuilder>();
            fakeRequestBuilder
                .Setup(service => service.BuildXmbCheckAvailabilityRequest("01550854", "123456", drsOrder, DateTime.Now, DateTime.Now.AddDays(7)))
                .Returns(xmbCheckAvailabilty);
            fakeRequestBuilder.Setup(service => service.BuildXmbOpenSessionRequest()).Returns(new xmbOpenSession());
            fakeRequestBuilder.Setup(service => service.BuildXmbCreateOrderRequest("01550854", "123456", drsOrder))
                .Returns(xmbCreateOrder);
            var fakeRepairRequestBuilder = new Mock<IHackneyRepairsServiceRequestBuilder>();
            AppointmentActions appointmentActions = new AppointmentActions(mockLogger.Object,
                mockAppointmentsService.Object, fakeRequestBuilder.Object, mockRepairsService.Object, fakeRepairRequestBuilder.Object);
            await Assert.ThrowsAsync<AppointmentServiceException>(async () =>
                await appointmentActions.GetAppointments("01550854"));
        }

        [Fact]
        public async Task get_appointmets_returns_list_of_appointments_if_the_work_order_already_exists_in_DRS()
        {
            var xmbResponse = new xmbCheckAvailabilityResponse
            {
                status = responseStatus.success,
                theSlots = new List<daySlotsInfo>
                {
                    new daySlotsInfo
                    {
                        day = new DateTime(2017, 10, 18, 00, 00, 00),
                        nonWorkingDay = false,
                        daySpecified = false,
                        weeklyDayOff = false,
                        slotsForDay = new List<slotInfo>
                        {
                            new slotInfo
                            {
                                available = availableValue.YES,
                                beginDate = new DateTime(2017, 10, 18, 10, 00, 00),
                                endDate = new DateTime(2017, 10, 18, 12, 00, 00),
                                bestSlot = true
                            },
                            new slotInfo
                            {
                                available = availableValue.YES,
                                beginDate = new DateTime(2017, 10, 18, 12, 00, 00),
                                endDate = new DateTime(2017, 10, 18, 14, 00, 00),
                                bestSlot = false
                            },
                            new slotInfo
                            {
                                available = availableValue.YES,
                                beginDate = new DateTime(2017, 10, 18, 14, 00, 00),
                                endDate = new DateTime(2017, 10, 18, 16, 00, 00),
                                bestSlot = false
                            }
                        }.ToArray()
                    }
                }.ToArray()
            };
            var response = new checkAvailabilityResponse(xmbResponse);
            var mockLogger = new Mock<ILoggerAdapter<AppointmentActions>>();
            var mockAppointmentsService = new Mock<IHackneyAppointmentsService>();
            mockAppointmentsService.Setup(service => service.GetAppointmentsForWorkOrderReference(It.IsAny<xmbCheckAvailability>()))
                .ReturnsAsync(response);
            var openSessionResponse = new openSessionResponse(
                new xmbOpenSessionResponse
                {
                    status = responseStatus.success,
                    sessionId = "123456"
                }
            );
            var closeSessionResponse = new closeSessionResponse(
                new xmbCloseSessionResponse
                {
                    status = responseStatus.success
                }
            );
            var drsOrder = new DrsOrder { wo_ref = "01550854" };
            var xmbCheckAvailabilty = new xmbCheckAvailability { theOrder = new order { primaryOrderNumber = "01550854" } };
            var xmbCreateOrder =
                new xmbCreateOrder { sessionId = "123456", theOrder = new order { primaryOrderNumber = "01550864" } };
            var createOrderResponse = new createOrderResponse
            {
                @return = new xmbCreateOrderResponse
                {
                    status = responseStatus.error,
                    errorMsg = "Unable to create order, an order already exists for the order number 01550854",
                    theOrder = new order { orderId = 123 }
                }
            };
            mockAppointmentsService.Setup(service => service.OpenSessionAsync(It.IsAny<xmbOpenSession>()))
                .ReturnsAsync(openSessionResponse);
            mockAppointmentsService.Setup(service => service.CloseSessionAsync(It.IsAny<xmbCloseSession>()))
                .ReturnsAsync(closeSessionResponse);
            mockAppointmentsService.Setup(service => service.CreateWorkOrderAsync(xmbCreateOrder))
                .ReturnsAsync(createOrderResponse);
            var mockRepairsService = new Mock<IHackneyRepairsService>();
            mockRepairsService.Setup(service => service.GetWorkOrderDetails("01550854")).ReturnsAsync(drsOrder);
            var fakeRequestBuilder = new Mock<IHackneyAppointmentsServiceRequestBuilder>();
            fakeRequestBuilder.Setup(service => service.BuildXmbCheckAvailabilityRequest("01550854", "123456", drsOrder, DateTime.Now, DateTime.Now.AddDays(7))).Returns(xmbCheckAvailabilty);
            fakeRequestBuilder.Setup(service => service.BuildXmbOpenSessionRequest()).Returns(new xmbOpenSession());
            fakeRequestBuilder.Setup(service => service.BuildXmbCreateOrderRequest("01550854", "123456", drsOrder))
                .Returns(xmbCreateOrder);
            var fakeRepairRequestBuilder = new Mock<IHackneyRepairsServiceRequestBuilder>();
            AppointmentActions appointmentActions = new AppointmentActions(mockLogger.Object, mockAppointmentsService.Object, fakeRequestBuilder.Object, mockRepairsService.Object, fakeRepairRequestBuilder.Object);
            var results = await appointmentActions.GetAppointments("01550854");
            Assert.Equal(results.Count, xmbResponse.theSlots[0].slotsForDay.Length);
        }

        [Fact]
        public async Task book_appointment_returns_success_result()
        {
            var scheduleBookingResponse = new scheduleBookingResponse(new xmbScheduleBookingResponse{ status = responseStatus.success});
            var mockLogger = new Mock<ILoggerAdapter<AppointmentActions>>();
            var mockAppointmentsService = new Mock<IHackneyAppointmentsService>();
            var openSessionResponse = new openSessionResponse(
                new xmbOpenSessionResponse
                {
                    status = responseStatus.success,
                    sessionId = "123456"
                }
            );
            var closeSessionResponse = new closeSessionResponse(
                new xmbCloseSessionResponse
                {
                    status = responseStatus.success
                }
            );
            var drsOrder = new DrsOrder { wo_ref = "01550854" };
            var xmbScheduleBooking = new xmbScheduleBooking
            {
                theBooking = new booking()
            };
            mockAppointmentsService.Setup(service => service.OpenSessionAsync(It.IsAny<xmbOpenSession>()))
                .ReturnsAsync(openSessionResponse);
            mockAppointmentsService.Setup(service => service.CloseSessionAsync(It.IsAny<xmbCloseSession>()))
                .ReturnsAsync(closeSessionResponse);
            mockAppointmentsService.Setup(service => service.ScheduleBookingAsync(xmbScheduleBooking))
                .ReturnsAsync(scheduleBookingResponse);
            mockAppointmentsService.Setup(service => service.SelectOrderAsync(It.IsAny<xmbSelectOrder>()))
                .ReturnsAsync(new selectOrderResponse(new xmbSelectOrderResponse
                {
                    status = responseStatus.success,
                    theOrders = new List<order>
                    {
                        new order
                        {
                            contract = "H01",
                            theBookings = new List<booking>
                            {
                                new booking
                                {
                                    bookingId = 123456
                                }
                            }.ToArray(),
                            theLocation= new location
                            {
                                locationId = "012345",
                                contract = "H01",
                                name = "An address",
                                address1 = "An address",
                                postCode = "Apostcode"
                            }
       
                        }
                    }.ToArray()
                }));
            var mockWorksOrderResponse = new RepairsService.WebResponse
                                {
                                    Success = true
                                };
            var mockRepairsService = new Mock<IHackneyRepairsService>();
            mockRepairsService.Setup(service => service.GetWorkOrderDetails("01550854")).ReturnsAsync(drsOrder);
            mockRepairsService.Setup(service => service.IssueOrderAsync(It.IsAny<RepairsService.WorksOrderRequest>()))
                .ReturnsAsync(mockWorksOrderResponse);
            var fakeRequestBuilder = new Mock<IHackneyAppointmentsServiceRequestBuilder>();
            fakeRequestBuilder.Setup(service => service.BuildXmbOpenSessionRequest()).Returns(new xmbOpenSession());
            fakeRequestBuilder.Setup(service => service.BuildXmbCloseSessionRequest("123456"))
                .Returns(new xmbCloseSession());
            fakeRequestBuilder.Setup(service => service.BuildXmbScheduleBookingRequest("01550854", "123456",
                    new DateTime(2017, 11, 21, 10, 00, 00), new DateTime(2017, 11, 21, 12, 00, 00), drsOrder))
                .Returns(xmbScheduleBooking);
            var fakeRepairRequestBuilder = new Mock<IHackneyRepairsServiceRequestBuilder>();
            AppointmentActions appointmentActions = new AppointmentActions(mockLogger.Object, mockAppointmentsService.Object, fakeRequestBuilder.Object, mockRepairsService.Object, fakeRepairRequestBuilder.Object);
            var result = await appointmentActions.BookAppointment("01550854", new DateTime(2017, 11, 21,10,00,00), new DateTime(2017, 11, 21, 12, 00, 00));
            Assert.Contains("{ beginDate = 2017-11-21T10:00:00Z, endDate = 2017-11-21T12:00:00Z }", result.ToString());
        }

        [Fact]
        public async Task book_appointment_raises_exception_when_service_responds_with_error()
        {
            var scheduleBookingResponse = new scheduleBookingResponse(new xmbScheduleBookingResponse { status = responseStatus.error });
            var mockLogger = new Mock<ILoggerAdapter<AppointmentActions>>();
            var mockAppointmentsService = new Mock<IHackneyAppointmentsService>();
            var openSessionResponse = new openSessionResponse(
                new xmbOpenSessionResponse
                {
                    status = responseStatus.success,
                    sessionId = "123456"
                }
            );
            var closeSessionResponse = new closeSessionResponse(
                new xmbCloseSessionResponse
                {
                    status = responseStatus.success
                }
            );
            var drsOrder = new DrsOrder { wo_ref = "01550854" };
            mockAppointmentsService.Setup(service => service.OpenSessionAsync(It.IsAny<xmbOpenSession>()))
                .ReturnsAsync(openSessionResponse);
            mockAppointmentsService.Setup(service => service.CloseSessionAsync(It.IsAny<xmbCloseSession>()))
                .ReturnsAsync(closeSessionResponse);
            mockAppointmentsService.Setup(service => service.ScheduleBookingAsync(It.IsAny<xmbScheduleBooking>()))
                .ReturnsAsync(scheduleBookingResponse);
            mockAppointmentsService.Setup(service => service.SelectOrderAsync(It.IsAny<xmbSelectOrder>()))
                .ReturnsAsync(new selectOrderResponse(new xmbSelectOrderResponse
                {
                    status = responseStatus.success,
                    theOrders = new List<order>
                    {
                        new order
                        {
                            orderId = 123,
                            theBookings = new List<booking>
                            {
                                new booking
                                {
                                    bookingId = 123456
                                }
                            }.ToArray()
                        }
                    }.ToArray()
                }));
            var mockRepairsService = new Mock<IHackneyRepairsService>();
            mockRepairsService.Setup(service => service.GetWorkOrderDetails("01550854")).ReturnsAsync(drsOrder);
            var fakeRequestBuilder = new Mock<IHackneyAppointmentsServiceRequestBuilder>();
            fakeRequestBuilder.Setup(service => service.BuildXmbOpenSessionRequest()).Returns(new xmbOpenSession());
            fakeRequestBuilder.Setup(service => service.BuildXmbCloseSessionRequest("123456"))
                .Returns(new xmbCloseSession());
            fakeRequestBuilder.Setup(service => service.BuildXmbScheduleBookingRequest("01550854", "123456",
                    new DateTime(2017, 11, 21, 10, 00, 00), new DateTime(2017, 11, 21, 12, 00, 00), drsOrder))
                .Returns(new xmbScheduleBooking{theBooking = new booking()});
            var fakeRepairRequestBuilder = new Mock<IHackneyRepairsServiceRequestBuilder>();
            AppointmentActions appointmentActions = new AppointmentActions(mockLogger.Object, mockAppointmentsService.Object, fakeRequestBuilder.Object, mockRepairsService.Object, fakeRepairRequestBuilder.Object);
            await Assert.ThrowsAsync<AppointmentServiceException>(async () => await appointmentActions.BookAppointment("01550854", new DateTime(2017, 11, 21, 10, 00, 00), new DateTime(2017, 11, 21, 12, 00, 00)));
        }

        [Fact]
        public async Task book_appointment_raises_an_exception_when_drs_service_responds_with_error_when_opening_session()
        {
            var scheduleBookingResponse = new scheduleBookingResponse(new xmbScheduleBookingResponse { status = responseStatus.success });
            var mockLogger = new Mock<ILoggerAdapter<AppointmentActions>>();
            var mockAppointmentsService = new Mock<IHackneyAppointmentsService>();
            var openSessionResponse = new openSessionResponse(
                new xmbOpenSessionResponse
                {
                    status = responseStatus.error,
                    sessionId = ""
                }
            );
            var closeSessionResponse = new closeSessionResponse(
                new xmbCloseSessionResponse
                {
                    status = responseStatus.success
                }
            );
            var drsOrder = new DrsOrder { wo_ref = "01550854" };
            mockAppointmentsService.Setup(service => service.OpenSessionAsync(It.IsAny<xmbOpenSession>()))
                .ReturnsAsync(openSessionResponse);
            mockAppointmentsService.Setup(service => service.CloseSessionAsync(It.IsAny<xmbCloseSession>()))
                .ReturnsAsync(closeSessionResponse);
            mockAppointmentsService.Setup(service => service.ScheduleBookingAsync(It.IsAny<xmbScheduleBooking>()))
                .ReturnsAsync(scheduleBookingResponse);
            var mockRepairsService = new Mock<IHackneyRepairsService>();
            mockRepairsService.Setup(service => service.GetWorkOrderDetails("01550854")).ReturnsAsync(drsOrder);
            var fakeRequestBuilder = new Mock<IHackneyAppointmentsServiceRequestBuilder>();
            fakeRequestBuilder.Setup(service => service.BuildXmbOpenSessionRequest()).Returns(new xmbOpenSession());
            fakeRequestBuilder.Setup(service => service.BuildXmbCloseSessionRequest("123456"))
                .Returns(new xmbCloseSession());
            fakeRequestBuilder.Setup(service => service.BuildXmbScheduleBookingRequest("01550854", "123456",
                    new DateTime(2017, 11, 21, 10, 00, 00), new DateTime(2017, 11, 21, 12, 00, 00), drsOrder))
                .Returns(new xmbScheduleBooking());
            var fakeRepairRequestBuilder = new Mock<IHackneyRepairsServiceRequestBuilder>();
            AppointmentActions appointmentActions = new AppointmentActions(mockLogger.Object, mockAppointmentsService.Object, fakeRequestBuilder.Object, mockRepairsService.Object, fakeRepairRequestBuilder.Object);
            await Assert.ThrowsAsync<AppointmentServiceException>(async () => await appointmentActions.BookAppointment("01550854", new DateTime(2017, 11, 21, 10, 00, 00), new DateTime(2017, 11, 21, 12, 00, 00)));
        }

        [Fact]
        public async Task book_appointment_raises_an_exception_when_drs_service_responds_with_error_when_closing_session()
        {
            var scheduleBookingResponse = new scheduleBookingResponse(new xmbScheduleBookingResponse { status = responseStatus.success });
            var mockLogger = new Mock<ILoggerAdapter<AppointmentActions>>();
            var mockAppointmentsService = new Mock<IHackneyAppointmentsService>();
            var openSessionResponse = new openSessionResponse(
                new xmbOpenSessionResponse
                {
                    status = responseStatus.success,
                    sessionId = "123456"
                }
            );
            var closeSessionResponse = new closeSessionResponse(
                new xmbCloseSessionResponse
                {
                    status = responseStatus.error
                }
            );
            var drsOrder = new DrsOrder { wo_ref = "01550854" };
            mockAppointmentsService.Setup(service => service.OpenSessionAsync(It.IsAny<xmbOpenSession>()))
                .ReturnsAsync(openSessionResponse);
            mockAppointmentsService.Setup(service => service.CloseSessionAsync(It.IsAny<xmbCloseSession>()))
                .ReturnsAsync(closeSessionResponse);
            mockAppointmentsService.Setup(service => service.ScheduleBookingAsync(It.IsAny<xmbScheduleBooking>()))
                .ReturnsAsync(scheduleBookingResponse);
            mockAppointmentsService.Setup(service => service.SelectOrderAsync(It.IsAny<xmbSelectOrder>()))
                .ReturnsAsync(new selectOrderResponse(new xmbSelectOrderResponse
                {
                    status = responseStatus.success,
                    theOrders = new List<order>
                    {
                        new order
                        {
                            orderId = 123,
                            theBookings = new List<booking>
                            {
                                new booking
                                {
                                    bookingId = 123456
                                }
                            }.ToArray()
                        }
                    }.ToArray()
                }));
            var mockRepairsService = new Mock<IHackneyRepairsService>();
            mockRepairsService.Setup(service => service.GetWorkOrderDetails("01550854")).ReturnsAsync(drsOrder);
            var fakeRequestBuilder = new Mock<IHackneyAppointmentsServiceRequestBuilder>();
            fakeRequestBuilder.Setup(service => service.BuildXmbOpenSessionRequest()).Returns(new xmbOpenSession());
            fakeRequestBuilder.Setup(service => service.BuildXmbCloseSessionRequest("123456"))
                .Returns(new xmbCloseSession());
            fakeRequestBuilder.Setup(service => service.BuildXmbScheduleBookingRequest("01550854", "123456",
                    new DateTime(2017, 11, 21, 10, 00, 00), new DateTime(2017, 11, 21, 12, 00, 00), drsOrder))
                .Returns(new xmbScheduleBooking{theBooking = new booking()});
            var fakeRepairRequestBuilder = new Mock<IHackneyRepairsServiceRequestBuilder>();
            AppointmentActions appointmentActions = new AppointmentActions(mockLogger.Object, mockAppointmentsService.Object, fakeRequestBuilder.Object, mockRepairsService.Object, fakeRepairRequestBuilder.Object);
            await Assert.ThrowsAsync<AppointmentServiceException>(async () => await appointmentActions.BookAppointment("01550854", new DateTime(2017, 11, 21, 10, 00, 00), new DateTime(2017, 11, 21, 12, 00, 00)));
        }
    }
}

