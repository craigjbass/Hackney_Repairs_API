using System;
using System.Configuration;
using System.Threading.Tasks;
using HackneyRepairs.Actions;
using HackneyRepairs.Interfaces;
using HackneyRepairs.Models;
using HackneyRepairs.PropertyService;

namespace HackneyRepairs.Services
{
    public class HackneyPropertyService : IHackneyPropertyService
    {
        private readonly PropertyServiceClient _client;
        private IUhtRepository _uhtRepository;
        private IUHWWarehouseRepository _uhWarehouseRepository;
        private ILoggerAdapter<PropertyActions> _logger;
        public HackneyPropertyService(IUhtRepository uhtRepository, IUHWWarehouseRepository uHWWarehouseRepository, ILoggerAdapter<PropertyActions> logger)
        {
            _client = new PropertyServiceClient();
            _uhtRepository = uhtRepository;
            _uhWarehouseRepository = uHWWarehouseRepository;
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

        public async Task<PropertySummary[]> GetPropertyListByPostCode(string post_code)
        {
            _logger.LogInformation($"HackneyPropertyService/GetPropertyListByPostCode(): Sent request to upstream data warehouse (Postcode: {post_code})");
            var response = await _uhWarehouseRepository.GetPropertyListByPostCode(post_code);
            _logger.LogInformation($"HackneyPropertyService/GetPropertyListByPostCode(): Received response from upstream data warehouse (Postcode: {post_code})");
            return response;
        }

        public async Task<PropertyDetails> GetPropertyBlockByRef(string reference)
        {
            PropertyDetails property = new PropertyDetails();
            _logger.LogInformation($"HackneyPropertyService/GetPropertyBlockByRef(): Sent request to upstream data warehouse (Property reference: {reference})");
            property = await _uhWarehouseRepository.GetPropertyBlockByReference(reference);
            _logger.LogInformation($"HackneyPropertyService/GetPropertyBlockByRef(): Received response from upstream data warehouse (Property reference: {reference})");
            return property;
        }
    }
}