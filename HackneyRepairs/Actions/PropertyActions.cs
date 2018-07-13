using System;
using System.Linq;
using System.Threading.Tasks;
using HackneyRepairs.Interfaces;
using HackneyRepairs.Models;
using HackneyRepairs.PropertyService;
namespace HackneyRepairs.Actions
{
    public class PropertyActions
    {
        private IHackneyPropertyService _service;
        private IHackneyPropertyServiceRequestBuilder _requestBuilder;
        private readonly ILoggerAdapter<PropertyActions> _logger;

        public PropertyActions(IHackneyPropertyService service, IHackneyPropertyServiceRequestBuilder requestBuilder, ILoggerAdapter<PropertyActions> logger)
        {
            _service = service;
            _requestBuilder = requestBuilder;
            _logger = logger;
        }

        public async Task<object> FindProperty(string postcode)
        {
            _logger.LogInformation($"Finding property by postcode: {postcode}");
            var request = _requestBuilder.BuildListByPostCodeRequest(postcode);
            try
            {
                var response = await _service.GetPropertyListByPostCode(request);
                if(response == null)
                {
                    _logger.LogError($"Finding property by postcode: {request} response not set to an instance of PropertySummary[].");
                    throw new MissingPropertyListException();                    
                }
                return new
                {
                    results = response.Select(buildProperty).ToArray()
                };
            }
            catch(Exception e)
            {
                _logger.LogError($"Finding property by postcode: {request} returned an error: {e.Message}");
                throw new PropertyServiceException();
            }
        }

        public async Task<object> FindPropertyDetailsByRef(string reference)
        {
            _logger.LogInformation($"Finding property by reference: {reference}");
            var request = _requestBuilder.BuildByPropertyRefRequest(reference);
            var response = await _service.GetPropertyByRefAsync(request);
            var maintainable = _service.GetMaintainable(reference).Result;
            if (!response.Success)
            {
                throw new PropertyServiceException();
            }
            if (response.Property == null)
            {
                throw new MissingPropertyException();
            }
            return new
            {
                address = response.Property.ShortAddress.Trim(),
                postcode = response.Property.PostCodeValue.Trim(),
                propertyReference = response.Property.Reference.Trim(),
                maintainable = maintainable
            };
        }

        public async Task<object> FindPropertyBlockDetailsByRef(string reference)
        {
            _logger.LogInformation($"Finding the block of a property by the property reference: {reference}");
            try
            {
                var response = await _service.GetPropertyBlockByRef(reference);
                if (response.PropertyReference == null)
                    return null;
                else
                return new
                {
                    address = response.ShortAddress.Trim(),
                    postcode = response.PostCodeValue.Trim(),
                    propertyReference = response.PropertyReference.Trim(),
                    maintainable = response.Maintainable
                };
            }
            catch(Exception e)
            {
                _logger.LogError($"Finding the block of a property by the property reference: {reference} returned an error: {e.Message}");
                throw e;
            }
        }

        public async Task<object> FindPropertyEstateDetailsByRef(string reference)
        {
            _logger.LogInformation($"Finding the estate of a property by the property reference: {reference}");
            try
            {
                var response = await _service.GetPropertyEstateByRef(reference);
                if (response.PropertyReference == null)
                {
                    return null;
                }
                else
                {
                    return new
                    {
                        address = response.ShortAddress.Trim(),
                        postcode = response.PostCodeValue.Trim(),
                        propertyReference = response.PropertyReference.Trim(),
                        maintainable = response.Maintainable
                    };
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Finding the estate of a property by the property reference: {reference} returned an error: {e.Message}");
                throw e;
            }
        }

        private object buildProperty(PropertySummary property)
        {
            return new
            {
                address = property.ShortAddress.Trim(),
                postcode = property.PostCodeValue.Trim(),
                propertyReference = property.PropertyReference.Trim()
            };
        }
    }

    public class MissingPropertyListException : System.Exception{}

    public class PropertyServiceException : System.Exception { }

    public class MissingPropertyException : System.Exception { }
}
