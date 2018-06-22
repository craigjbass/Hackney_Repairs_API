using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using HackneyRepairs.Models;
using HackneyRepairs.Interfaces;
using HackneyRepairs.Factories;
using HackneyRepairs.Actions;
using HackneyRepairs.Formatters;
using HackneyRepairs.Services;
using HackneyRepairs.Validators;

namespace HackneyRepairs.Controllers
{
    [Produces("application/json")]
    [Route("v1/properties")]
    public class PropertiesController : Controller
    {
        private IHackneyPropertyService _propertyService;
        private IHackneyPropertyServiceRequestBuilder _propertyServiceRequestBuilder;
        private IPostcodeValidator _postcodeValidator;
        private ILoggerAdapter<PropertyActions> _loggerAdapter;
        private HackneyConfigurationBuilder _configBuilder;

        public PropertiesController(ILoggerAdapter<PropertyActions> loggerAdapter, IUhtRepository uhtRepository)
        {
            HackneyPropertyServiceFactory factory = new HackneyPropertyServiceFactory();
            _configBuilder = new HackneyConfigurationBuilder((Hashtable)Environment.GetEnvironmentVariables(), ConfigurationManager.AppSettings);
            _propertyService = factory.build(uhtRepository, loggerAdapter);
            _propertyServiceRequestBuilder = new HackneyPropertyServiceRequestBuilder(_configBuilder.getConfiguration(), new PostcodeFormatter());
            _postcodeValidator = new PostcodeValidator();
            _loggerAdapter = loggerAdapter;
        }

        // GET properties
        /// <summary>
        /// Gets a property or properties for a particular postcode
        /// </summary>
        /// <param name="postcode">The post code of the propterty being requested</param>
        /// <returns>A list of properties matching the specified post code</returns>
        /// <response code="200">Returns the list of properties</response>
        /// <response code="400">If a post code is not provided</response>   
        /// <response code="500">If any errors are encountered</response>   
        [HttpGet]
        public async Task<JsonResult> Get([FromQuery]string postcode)
        {
            try
            {
                if (_postcodeValidator.Validate(postcode))
                {
                    PropertyActions actions = new PropertyActions(_propertyService, _propertyServiceRequestBuilder, _loggerAdapter);
                    var json = Json(await actions.FindProperty(postcode));
                    json.StatusCode = 200;
                    json.ContentType = "application/json";
                    return json;
                }
                else
                {
                    var errors = new List<ApiErrorMessage>
                    {
                        new ApiErrorMessage
                        {
                            developerMessage = "Invalid parameter - postcode",
                            userMessage = "Please provide a valid post code"
                        }
                    };
                    var json = Json(errors);
                    json.StatusCode = 400;
                    return json;
                }
            }

            catch (Exception e)
            {
                var errors = new List<ApiErrorMessage>
                {
                    new ApiErrorMessage
                    {
                        developerMessage = e.Message,
                        userMessage = "We had some problems processing your request"
                    }
                };
                var json = Json(errors);
                json.StatusCode = 500;
                return json;
            }
        }

        // GET property details by reference
        /// <summary>
        /// Gets the details of a property by a given reference number
        /// </summary>
        /// <param name="reference">The reference number of the requested property</param>
        /// <returns>Details of the requested property</returns>
        [HttpGet("{reference}")]
        public async Task<JsonResult> GetByReference(string reference)
        {
            try
            {
                PropertyActions actions = new PropertyActions(_propertyService, _propertyServiceRequestBuilder, _loggerAdapter);
                var json = Json(await actions.FindPropertyDetailsByRef(reference));
                json.StatusCode = 200;
                json.ContentType = "application/json";
                return json;
            }
            catch (MissingPropertyException ex)
            {
                var errors = new List<ApiErrorMessage>()
                {
                    new ApiErrorMessage
                    {
                        developerMessage = ex.Message,
                        userMessage = "Resource identification error"
                    }
                };
                var jsonResponse = Json(errors);
                jsonResponse.StatusCode = 404;
                return jsonResponse;
            }
            catch (Exception e)
            {
                var errors = new List<ApiErrorMessage>()
                {
                    new ApiErrorMessage
                    {
                        developerMessage = e.Message,
                        userMessage = "Internal Error"
                    }
                };
                var jsonResponse = Json(errors);
                jsonResponse.StatusCode = 500;
                return jsonResponse;
            }
        }

    }
}