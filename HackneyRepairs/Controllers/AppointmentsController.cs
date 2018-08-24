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
using HackneyRepairs.Repository;

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
			ILoggerAdapter<HackneyAppointmentsServiceRequestBuilder> requestBuildLoggerAdapter, ILoggerAdapter<RepairsActions> repairsLoggerAdapter,
                                      IDRSRepository drsRepository)
		{
			var serviceFactory = new HackneyAppointmentServiceFactory();
			_configBuilder = new HackneyConfigurationBuilder((Hashtable)Environment.GetEnvironmentVariables(), ConfigurationManager.AppSettings);
            _appointmentsService = serviceFactory.build(loggerAdapter, uhtRepository, drsRepository);
			var factory = new HackneyRepairsServiceFactory();
			_repairsService = factory.build(uhtRepository, uhwRepository, repairsLoggerAdapter);
			_loggerAdapter = loggerAdapter;
			_serviceRequestBuilder = new HackneyAppointmentsServiceRequestBuilder(_configBuilder.getConfiguration(), requestBuildLoggerAdapter);
			_scheduleBookingRequestValidator = new ScheduleBookingRequestValidator(_repairsService);
			_repairsServiceRequestBuilder = new HackneyRepairsServiceRequestBuilder(_configBuilder.getConfiguration());
		}

		// GET available appointments for a Universal Housing work order
		/// <summary>
		/// Returns available appointments for a Universal Housing work order
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
					var appointmentsActions = new AppointmentActions(_loggerAdapter, _appointmentsService, _serviceRequestBuilder, _repairsService, _repairsServiceRequestBuilder, _configBuilder.getConfiguration());
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

		/// <summary>
		/// Creates an appointment
		/// </summary>
		/// <param name="workOrderReference">The reference number of the work order for the appointment</param>
		/// <param name="appointment">Details of the appointment to be booked</param>
		/// <returns>A JSON object for a successfully created appointment</returns>
		/// <response code="200">A successfully created repair request</response>
		[HttpPost]
		[Route("v1/work_orders/{workOrderReference}/appointments")]
		public async Task<JsonResult> Post(string workOrderReference, [FromBody]ScheduleAppointmentRequest request)
		{
			try
			{
				var validationResult = _scheduleBookingRequestValidator.Validate(workOrderReference, request);
				if (validationResult.Valid)
				{
					var appointmentsActions = new AppointmentActions(_loggerAdapter, _appointmentsService,
																	 _serviceRequestBuilder, _repairsService, _repairsServiceRequestBuilder, _configBuilder.getConfiguration());
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
      
		// GET all appointments booked appointments by work order reference 
        /// <summary>
        /// Returns all apointments for a work order
        /// </summary>
        /// <param name="workOrderReference">UH work order reference</param>
        /// <returns>A list of UHT appointment entities</returns>
        /// <response code="200">Returns a list of appointments for a work order reference</response>
        /// <response code="404">If there are no appointments found for the work orders reference</response>   
        /// <response code="500">If any errors are encountered</response>
		[HttpGet("v1/work_orders/{workOrderReference}/appointments")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
		public async Task<JsonResult> GetAppointmentsByWorkOrderReference(string workOrderReference)
        {
			var appointmentsActions = new AppointmentActions(_loggerAdapter, _appointmentsService, _serviceRequestBuilder, _repairsService, _repairsServiceRequestBuilder, _configBuilder.getConfiguration());
            IEnumerable<DetailedAppointment> result;
            try
            {
				result = await appointmentsActions.GetAppointmentsByWorkOrderReference(workOrderReference);
                var json = Json(result);
                json.StatusCode = 200;
                return json;
            }
            catch(MissingAppointmentsException)
            {
                return new JsonResult(new string[0]);
            }
            catch (InvalidWorkOrderInUHException ex)
            {
                var error = new ApiErrorMessage
                {
                    developerMessage = ex.Message,
                    userMessage = @"workOrderReference not found"
                };
                var jsonResponse = Json(error);
                jsonResponse.StatusCode = 404;
                return jsonResponse;
            }
            catch (UhtRepositoryException ex)
            {
                var error = new ApiErrorMessage
                {
                    developerMessage = ex.Message,
                    userMessage = @"We had issues with connecting to the data source."
                };
                var jsonResponse = Json(error);
                jsonResponse.StatusCode = 500;
                return jsonResponse;
            }
            catch (Exception ex)
            {
                var error = new ApiErrorMessage
                {
                    developerMessage = ex.Message,
                    userMessage = @"We had issues processing your request"
                };
                var jsonResponse = Json(error);
                jsonResponse.StatusCode = 500;
                return jsonResponse;
            }
        }
	}
}
