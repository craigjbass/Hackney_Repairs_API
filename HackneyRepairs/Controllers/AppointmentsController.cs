using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using HackneyRepairs.Factories;
using HackneyRepairs.Interfaces;
using HackneyRepairs.Models;
using HackneyRepairs.Actions;
using HackneyRepairs.Formatters;
using HackneyRepairs.Services;
using HackneyRepairs.Validators;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace HackneyRepairs.Controllers
{
    [Produces("application/json")]
    public class AppointmentsController : Controller
    {
        private IHackneyAppointmentsService _appointmentsService;
        private IHackneyRepairsService _repairsService;
        private ILoggerAdapter<AppointmentActions> _loggerAdapter;
        private IHackneyAppointmentsServiceRequestBuilder _serviceRequestBuilder;
        private IHackneyRepairsServiceRequestBuilder _repairsServiceRequestBuilder;
        private IScheduleBookingRequestValidator _scheduleBookingRequestValidator;
        private HackneyConfigurationBuilder _configBuilder;

        public AppointmentsController(ILoggerAdapter<AppointmentActions> loggerAdapter, IUhtRepository uhtRepository, IUhwRepository uhwRepository,
            ILoggerAdapter<HackneyAppointmentsServiceRequestBuilder> requestBuildLoggerAdapter, ILoggerAdapter<RepairsActions> repairsLoggerAdapter)
        {
            var serviceFactory = new HackneyAppointmentServiceFactory();
            _configBuilder = new HackneyConfigurationBuilder((Hashtable)Environment.GetEnvironmentVariables(), ConfigurationManager.AppSettings);
            _appointmentsService = serviceFactory.build(loggerAdapter);
            var factory = new HackneyRepairsServiceFactory();
            _repairsService = factory.build(uhtRepository, uhwRepository, repairsLoggerAdapter);
            _loggerAdapter = loggerAdapter;
            _serviceRequestBuilder = new HackneyAppointmentsServiceRequestBuilder(_configBuilder.getConfiguration(), requestBuildLoggerAdapter);
            _scheduleBookingRequestValidator = new ScheduleBookingRequestValidator(_repairsService);
            _repairsServiceRequestBuilder = new HackneyRepairsServiceRequestBuilder(_configBuilder.getConfiguration());
        }

        // GET available appointments
        /// <summary>
        /// Retrieves available appointments
        /// </summary>
        /// <param name="workorderreference">The work order reference for which to provide available appointments</param>
        /// <returns>A list of available appointments</returns>
        /// <response code="200">Returns the list of available appointments</response>
        /// <response code="400">If no valid work order reference is provided</response>   
        /// <response code="500">If any errors are encountered</response>   
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [Route("v1/work_orders/{workorderreference}/available_appointments")]
        public async Task<JsonResult> Get(string workorderreference)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(workorderreference))
                {
                    var errors = new List<ApiErrorMessage>
                    {
                        new ApiErrorMessage
                        {
                            developerMessage = "Invalid parameter - workorderreference",
                            userMessage = "Please provide a valid work order reference"
                        }
                    };
                    var json = Json(errors);
                    json.StatusCode = 400;
                    return json;
                }
                else
                {
                    var appointmentsActions = new AppointmentActions(_loggerAdapter, _appointmentsService, _serviceRequestBuilder, _repairsService, _repairsServiceRequestBuilder,_configBuilder.getConfiguration());
                    var response = await appointmentsActions.GetAppointments(workorderreference);
                    var json = Json(new { results = response.ToList().FormatAppointmentsDaySlots() });
                    json.StatusCode = 200;
                    json.ContentType = "application/json";
                    return json;
                }
            }
            catch (NoAvailableAppointmentsException)
            {
                var data = new List<string>();
                var json = Json(new { results = data });
                json.StatusCode = 200;
                json.ContentType = "application/json";
                return json;
            }
            catch (Exception ex)
            {
                var errors = new List<ApiErrorMessage>
                {
                    new ApiErrorMessage
                    {
                        developerMessage = ex.Message,
                        userMessage = "We had some problems processing your request"
                    }
                };
                var json = Json(errors);
                json.StatusCode = 500;
                return json;
            }
        }

        /// <summary>
        /// Creates an appointment
        /// </summary>
        /// <param name="workorderreference">The reference number of the work order for the appointment</param>
        /// <param name="appointment">Details of the appointment to be booked</param>
        /// <returns>A JSON object for a successfully created appointment</returns>
        /// <response code="200">A successfully created repair request</response>
        [HttpPost]
        [Route("v1/work_orders/{workorderreference}/appointments")]
        public async Task<JsonResult> Post(string workorderreference, [FromBody]ScheduleAppointmentRequest request)
        {
            try
            {
                var validationResult = _scheduleBookingRequestValidator.Validate(workorderreference, request);
                if (validationResult.Valid)
                {
                    var appointmentsActions = new AppointmentActions(_loggerAdapter, _appointmentsService,
                                                                     _serviceRequestBuilder, _repairsService, _repairsServiceRequestBuilder, _configBuilder.getConfiguration());
                    var result = await appointmentsActions.BookAppointment(workorderreference,
                        DateTime.Parse(request.BeginDate),
                        DateTime.Parse(request.EndDate));
                    var json = Json(result);
                    json.StatusCode = 200;
                    json.ContentType = "application/json";
                    return json;
                }
                else
                {
                    var errors = validationResult.ErrorMessages.Select(error => new ApiErrorMessage
                    {
                        developerMessage = error,
                        userMessage = error
                    }).ToList();
                    var jsonResponse = Json(errors);
                    jsonResponse.StatusCode = 400;
                    return jsonResponse;
                }
            }
            catch (Exception e)
            {
                var errorMessage = new ApiErrorMessage
                {
                    developerMessage = e.Message,
                    userMessage = "We had some problems processing your request"
                };
                var jsonResponse = Json(errorMessage);
                jsonResponse.StatusCode = 500;
                return jsonResponse;
            }
        }

        [HttpGet]
        [Route("v1/work_orders/{workOrderReference}/appointments")]
        public async Task<JsonResult> GetAppointment(string workOrderReference)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(workOrderReference))
                {
                    var errors = new List<ApiErrorMessage>
                    {
                        new ApiErrorMessage
                        {
                            developerMessage = "Invalid parameter - workorderreference",
                            userMessage = "Please provide a valid work order reference"
                        }
                    };
                    var json = Json(errors);
                    json.StatusCode = 400;
                    return json;
                }
                else
                {
                    var appointmentsActions = new AppointmentActions(_loggerAdapter, _appointmentsService, _serviceRequestBuilder, _repairsService, _repairsServiceRequestBuilder, _configBuilder.getConfiguration());
                    var response = await appointmentsActions.GetAppointmentForWorksOrder(workOrderReference);
                    var json = Json(response);
                    json.StatusCode = 200;
                    json.ContentType = "application/json";
                    return json;
                }
            }
            catch (Exception ex)
            {
                var errors = new List<ApiErrorMessage>
                {
                    new ApiErrorMessage
                    {
                        developerMessage = ex.Message,
                        userMessage = "We had some problems processing your request"
                    }
                };
                var json = Json(errors);
                json.StatusCode = 500;
                return json;
            }
        }
    }
}
