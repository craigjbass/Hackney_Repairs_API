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
            _configBuilder = new HackneyConfigurationBuilder((Dictionary<string, string>)Environment.GetEnvironmentVariables(), ConfigurationManager.AppSettings);
            _appointmentsService = serviceFactory.build(loggerAdapter);
            var factory = new HackneyRepairsServiceFactory();
            _repairsService = factory.build(uhtRepository, uhwRepository, repairsLoggerAdapter);
            _loggerAdapter = loggerAdapter;
            _serviceRequestBuilder = new HackneyAppointmentsServiceRequestBuilder(_configBuilder.getConfiguration(), requestBuildLoggerAdapter);
            _scheduleBookingRequestValidator = new ScheduleBookingRequestValidator(_repairsService);
            _repairsServiceRequestBuilder = new HackneyRepairsServiceRequestBuilder(_configBuilder.getConfiguration());
        }

        [HttpGet]
        [Route("v1/work_orders/{workOrderReference}/available_appointments")]
        public async Task<JsonResult> Get(string workOrderReference)
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
                    var appointmentsActions = new AppointmentActions(_loggerAdapter, _appointmentsService, _serviceRequestBuilder, _repairsService, _repairsServiceRequestBuilder);
                    var response = await appointmentsActions.GetAppointments(workOrderReference);
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

        [HttpPost]
        [Route("v1/work_orders/{workOrderReference}/[controller]")]
        public async Task<JsonResult> Post(string workOrderReference, [FromBody]ScheduleAppointmentRequest request)
        {
            try
            {
                var validationResult = _scheduleBookingRequestValidator.Validate(workOrderReference, request);
                if (validationResult.Valid)
                {
                    var appointmentsActions = new AppointmentActions(_loggerAdapter, _appointmentsService,
                        _serviceRequestBuilder, _repairsService, _repairsServiceRequestBuilder);
                    var result = await appointmentsActions.BookAppointment(workOrderReference,
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
        [Route("v1/work_orders/{workOrderReference}/[controller]")]
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
                    var appointmentsActions = new AppointmentActions(_loggerAdapter, _appointmentsService, _serviceRequestBuilder, _repairsService, _repairsServiceRequestBuilder);
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
