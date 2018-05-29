using System;
using System.Configuration;
using System.Threading.Tasks;
using HackneyRepairs.Actions;
using HackneyRepairs.Interfaces;
using HackneyRepairs.PropertyService;

namespace HackneyRepairs.Services
{
    public class HackneyPropertyService : IHackneyPropertyService
    {
        private readonly PropertyServiceClient _client;
        private IUhtRepository _uhtRepository;
        private ILoggerAdapter<PropertyActions> _logger;
        public HackneyPropertyService(IUhtRepository uhtRepository, ILoggerAdapter<PropertyActions> logger)
        {
            _client = new PropertyServiceClient();
            _uhtRepository = uhtRepository;
            _logger = logger;
        }

        public Task<PropertyInfoResponse> GetPropertyListByPostCodeAsync(ListByPostCodeRequest request)
        {
            _logger.LogInformation($"HackneyPropertyService/GetPropertyListByPostCodeAsync(): Sent request to upstream PropertyServiceClient (Postcode: {request.PostCode})");
            var response = _client.GetPropertyListByPostCodeAsync(request);
            _logger.LogInformation($"HackneyPropertyService/GetPropertyListByPostCodeAsync(): Received response from upstream PropertyServiceClient (Postcode: {request.PostCode})");
            return response;
        }

        public Task<PropertyGetResponse> GetPropertyByRefAsync(ByPropertyRefRequest request)
        {
            _logger.LogInformation($"HackneyPropertyService/GetPropertyByRefAsync(): Sent request to upstream PropertyServiceClient (Property reference: {request.PropertyReference})");
            var response = _client.GetPropertyByRefAsync(request);
            _logger.LogInformation($"HackneyPropertyService/GetPropertyByRefAsync(): Received response from upstream PropertyServiceClient (Property reference: {request.PropertyReference})");
            return response;
        }

        public async Task<bool> GetMaintainable(string reference)
        {
            _logger.LogInformation($"HackneyPropertyService/GetMaintainable(): Sent request to upstream PropertyServiceClient (Reference: {reference})");
            var response = await _uhtRepository.GetMaintainableFlag(reference);
            _logger.LogInformation($"HackneyPropertyService/GetMaintainable(): Received response from upstream PropertyServiceClient (Reference: {reference})");
            return response;
        }
    }
}