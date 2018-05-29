using System.Linq;
using System.Threading.Tasks;
using HackneyRepairs.Interfaces;
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
            var response = await _service.GetPropertyListByPostCodeAsync(request);
            if (!response.Success)
            {
                throw new PropertyServiceException();
            }
            var properties = response.PropertyList;
            if (properties == null)
            {
                throw new MissingPropertyListException();
            }
            return new
            {
                results = properties.Select(buildProperty).ToArray()
            };
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
