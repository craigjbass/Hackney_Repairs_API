using System.Threading.Tasks;
using HackneyRepairs.Actions;
using Moq;
using Xunit;
using System.Text;
using HackneyRepairs.PropertyService;
using HackneyRepairs.Interfaces;
using Newtonsoft.Json;
using HackneyRepairs.Models;

namespace HackneyRepairs.Tests.Actions
{
    public class PropertyActionsTest
    {
        [Fact]
        public async Task find_properties_returns_a_list_of_properties()
        {
            var mockLogger = new Mock<ILoggerAdapter<PropertyActions>>();
            var property1 = new PropertySummary()
            {
                ShortAddress = "Front Office, Robert House, 6 - 15 Florfield Road",
                PostCodeValue = "E8 1DT",
                PropertyReference = "1/43453543"
            };
            var property2 = new PropertySummary()
            {
                ShortAddress = "Maurice Bishop House, 17 Reading Lane",
                PostCodeValue = "E8 1DT",
                PropertyReference = "2/32453245"
            };
            var PropertyList = new PropertySummary[2];
            PropertyList[0] = property1;
            PropertyList[1] = property2;
            var fakeService = new Mock<IHackneyPropertyService>();
            fakeService.Setup(service => service.GetPropertyListByPostCode("E8 1DT"))
                .ReturnsAsync(PropertyList);
            var fakeRequestBuilder = new Mock<IHackneyPropertyServiceRequestBuilder>();
            fakeRequestBuilder.Setup(service => service.BuildListByPostCodeRequest("E8 1DT"))
                .Returns("E8 1DT");
            PropertyActions propertyActions = new PropertyActions(fakeService.Object, fakeRequestBuilder.Object, mockLogger.Object);
            var results = await propertyActions.FindProperty("E8 1DT");
            var outputproperty1 = new {
                address = "Front Office, Robert House, 6 - 15 Florfield Road",
                postcode = "E8 1DT",
                propertyReference = "1/43453543"
            };
            var outputproperty2 = new
            {
                address = "Maurice Bishop House, 17 Reading Lane",
                postcode = "E8 1DT",
                propertyReference = "2/32453245"
            };
            var properties = new object[2];
            properties[0] = outputproperty1;
            properties[1] = outputproperty2;
            var json = new {results = properties};
            Assert.Equal(JsonConvert.SerializeObject(json), JsonConvert.SerializeObject(results));
        }

        [Fact]
        public async Task find_properties_returns_an_empty_response_when_no_matches()
        {
            var mockLogger = new Mock<ILoggerAdapter<PropertyActions>>();
            var PropertyList = new PropertySummary[0];
            var fakeService = new Mock<IHackneyPropertyService>();
            fakeService.Setup(service => service.GetPropertyListByPostCode(It.IsAny<string>()))
                .ReturnsAsync(PropertyList);
            var fakeRequestBuilder = new Mock<IHackneyPropertyServiceRequestBuilder>();
            fakeRequestBuilder.Setup(service => service.BuildListByPostCodeRequest("E8 2LN")).Returns(string.Empty);
            PropertyActions propertyActions = new PropertyActions(fakeService.Object, fakeRequestBuilder.Object, mockLogger.Object);
            var results = await propertyActions.FindProperty("E8 2LN");
            var properties = new object[0];
            var json = new {results = properties};
            Assert.Equal(JsonConvert.SerializeObject(json), JsonConvert.SerializeObject(results));
        }

        [Fact]
        public async Task find_properties_raises_an_exception_if_the_property_list_is_missing()
        {
            var mockLogger = new Mock<ILoggerAdapter<PropertyActions>>();
            var fakeService = new Mock<IHackneyPropertyService>();
            PropertySummary[] response = null;
            fakeService.Setup(service => service.GetPropertyListByPostCode(It.IsAny<string>()))
                .ReturnsAsync(response);
            var fakeRequestBuilder = new Mock<IHackneyPropertyServiceRequestBuilder>();
            PropertyActions propertyActions = new PropertyActions(fakeService.Object, fakeRequestBuilder.Object, mockLogger.Object);
            await Assert.ThrowsAsync<PropertyServiceException>(async () => await propertyActions.FindProperty("E8 2LN"));
        }

        [Fact]
        public async Task get_property_details_by_reference_returns_a_property_object_for_a_valid_request()
        {
            var mockLogger = new Mock<ILoggerAdapter<PropertyActions>>();
            var request = new ByPropertyRefRequest();
            var response = new PropertyGetResponse()
            {
                Success = true,
                Property = new PropertyDto()
                {
                    ShortAddress = "Front Office, Robert House, 6 - 15 Florfield Road",
                    PostCodeValue = "E8 1DT",
                    Reference = "43453543"
                }
            };
            var fakeService = new Mock<IHackneyPropertyService>();
            fakeService.Setup(service => service.GetPropertyByRefAsync(request))
                .ReturnsAsync(response);
            var fakeRequestBuilder = new Mock<IHackneyPropertyServiceRequestBuilder>();
            fakeRequestBuilder.Setup(service => service.BuildByPropertyRefRequest("43453543")).Returns(request);
            PropertyActions propertyActions = new PropertyActions(fakeService.Object, fakeRequestBuilder.Object, mockLogger.Object);
            var results = await propertyActions.FindPropertyDetailsByRef("43453543");
            var property = new
            {
                address = "Front Office, Robert House, 6 - 15 Florfield Road",
                postcode = "E8 1DT",
                propertyReference = "43453543",
                maintainable = false
            };
            Assert.Equal(property, results);
        }

        [Fact]
        public async Task get_property_details_by_reference_raises_an_exception_if_the_property_is_missing()
        {
            var mockLogger = new Mock<ILoggerAdapter<PropertyActions>>();
            var request = new ByPropertyRefRequest();
            var response = new PropertyGetResponse { Success = true };
            var fakeService = new Mock<IHackneyPropertyService>();
            fakeService.Setup(service => service.GetPropertyByRefAsync(It.IsAny<ByPropertyRefRequest>()))
                .ReturnsAsync(response);
            var fakeRequestBuilder = new Mock<IHackneyPropertyServiceRequestBuilder>();
            fakeRequestBuilder.Setup(service => service.BuildByPropertyRefRequest("52525252534")).Returns(request);
            PropertyActions propertyActions = new PropertyActions(fakeService.Object, fakeRequestBuilder.Object, mockLogger.Object);
            await Assert.ThrowsAsync<HackneyRepairs.Actions.MissingPropertyException>(async () => await propertyActions.FindPropertyDetailsByRef("52525252534"));
        }

        [Fact]
        public async Task get_property_details_by_reference_raises_an_exception_if_the_service_responds_with_an_error()
        {
            var mockLogger = new Mock<ILoggerAdapter<PropertyActions>>();
            var request = new ByPropertyRefRequest();
            var response = new PropertyGetResponse { Success = false, Property = new PropertyDto() };
            var fakeService = new Mock<IHackneyPropertyService>();
            fakeService.Setup(service => service.GetPropertyByRefAsync(It.IsAny<ByPropertyRefRequest>()))
                .ReturnsAsync(response);
            var fakeRequestBuilder = new Mock<IHackneyPropertyServiceRequestBuilder>();
            fakeRequestBuilder.Setup(service => service.BuildByPropertyRefRequest("525252525")).Returns(request);
            PropertyActions propertyActions = new PropertyActions(fakeService.Object, fakeRequestBuilder.Object, mockLogger.Object);
            await Assert.ThrowsAsync<HackneyRepairs.Actions.PropertyServiceException>(async () => await propertyActions.FindPropertyDetailsByRef("525252525"));
        }

        [Fact]
        public async Task get_property_block_details_by_reference_returns_a_property_object_for_a_valid_request()
        {
            var mockLogger = new Mock<ILoggerAdapter<PropertyActions>>();
            var response = new PropertyDetails()
            {
                ShortAddress = "Front Office Block, Robert House, 6 - 15 Florfield Road",
                PostCodeValue = "E8 1DT",
                PropertyReference = "43453543",
                Maintainable = true
            };
            var fakeService = new Mock<IHackneyPropertyService>();
            fakeService.Setup(service => service.GetPropertyBlockByRef("43453543"))
                .ReturnsAsync(response);
            var fakeRequestBuilder = new Mock<IHackneyPropertyServiceRequestBuilder>();
            PropertyActions propertyActions = new PropertyActions(fakeService.Object, fakeRequestBuilder.Object, mockLogger.Object);
            var results = await propertyActions.FindPropertyBlockDetailsByRef("43453543");
            var property = new
            {
                address = "Front Office Block, Robert House, 6 - 15 Florfield Road",
                postcode = "E8 1DT",
                propertyReference = "43453543",
                maintainable = true
            };
            Assert.Equal(property, results);
        }

        [Fact]
        public async Task get_property_block_details_by_reference_returns_an_empty_property_details_object_if_the_property_is_missing()
        {
            var mockLogger = new Mock<ILoggerAdapter<PropertyActions>>();
            var response = new PropertyDetails();
            var fakeService = new Mock<IHackneyPropertyService>();
            fakeService.Setup(service => service.GetPropertyBlockByRef("52525252534"))
                .ReturnsAsync(response);
            var fakeRequestBuilder = new Mock<IHackneyPropertyServiceRequestBuilder>();
            PropertyActions propertyActions = new PropertyActions(fakeService.Object, fakeRequestBuilder.Object, mockLogger.Object);
            Assert.Equal(response.PropertyReference, await propertyActions.FindPropertyBlockDetailsByRef("52525252534"));
        }

        [Fact]
        public async Task get_property_block_details_by_reference_raises_an_exception_if_the_service_responds_with_an_error()
        {
            var mockLogger = new Mock<ILoggerAdapter<PropertyActions>>();
            var request = new ByPropertyRefRequest();
            var fakeService = new Mock<IHackneyPropertyService>();
            fakeService.Setup(service => service.GetPropertyBlockByRef("525252525"))
                       .ThrowsAsync(new HackneyRepairs.Repository.UHWWarehouseRepositoryException());
            var fakeRequestBuilder = new Mock<IHackneyPropertyServiceRequestBuilder>();
            PropertyActions propertyActions = new PropertyActions(fakeService.Object, fakeRequestBuilder.Object, mockLogger.Object);
            await Assert.ThrowsAsync<HackneyRepairs.Repository.UHWWarehouseRepositoryException>(async () => await propertyActions.FindPropertyBlockDetailsByRef("525252525"));
        }

        [Fact]
        public async Task get_property_estate_details_by_reference_returns_a_property_object_for_a_valid_request()
        {
            var mockLogger = new Mock<ILoggerAdapter<PropertyActions>>();
            var response = new PropertyDetails()
            {
                ShortAddress = "Front Office Estate, Robert House, 6 - 15 Florfield Road",
                PostCodeValue = "E8 1DT",
                PropertyReference = "43453543",
                Maintainable = true
            };
            var fakeService = new Mock<IHackneyPropertyService>();
            fakeService.Setup(service => service.GetPropertyEstateByRef("43453543"))
                .ReturnsAsync(response);
            var fakeRequestBuilder = new Mock<IHackneyPropertyServiceRequestBuilder>();
            PropertyActions propertyActions = new PropertyActions(fakeService.Object, fakeRequestBuilder.Object, mockLogger.Object);
            var results = await propertyActions.FindPropertyEstateDetailsByRef("43453543");
            var property = new
            {
                address = "Front Office Estate, Robert House, 6 - 15 Florfield Road",
                postcode = "E8 1DT",
                propertyReference = "43453543",
                maintainable = true
            };
            Assert.Equal(property, results);
        }

        [Fact]
        public async Task get_property_estate_details_by_reference_returns_an_empty_property_details_object_if_the_property_is_missing()
        {
            var mockLogger = new Mock<ILoggerAdapter<PropertyActions>>();
            var response = new PropertyDetails();
            var fakeService = new Mock<IHackneyPropertyService>();
            fakeService.Setup(service => service.GetPropertyEstateByRef("52525252534"))
                .ReturnsAsync(response);
            var fakeRequestBuilder = new Mock<IHackneyPropertyServiceRequestBuilder>();
            PropertyActions propertyActions = new PropertyActions(fakeService.Object, fakeRequestBuilder.Object, mockLogger.Object);
            Assert.Equal(response.PropertyReference, await propertyActions.FindPropertyEstateDetailsByRef("52525252534"));
        }

        [Fact]
        public async Task get_property_estate_details_by_reference_raises_an_exception_if_the_service_responds_with_an_error()
        {
            var mockLogger = new Mock<ILoggerAdapter<PropertyActions>>();
            var request = new ByPropertyRefRequest();
            var fakeService = new Mock<IHackneyPropertyService>();
            fakeService.Setup(service => service.GetPropertyEstateByRef("525252525"))
                       .ThrowsAsync(new HackneyRepairs.Repository.UHWWarehouseRepositoryException());
            var fakeRequestBuilder = new Mock<IHackneyPropertyServiceRequestBuilder>();
            PropertyActions propertyActions = new PropertyActions(fakeService.Object, fakeRequestBuilder.Object, mockLogger.Object);
            await Assert.ThrowsAsync<HackneyRepairs.Repository.UHWWarehouseRepositoryException>(async () => await propertyActions.FindPropertyEstateDetailsByRef("525252525"));
        }
    }
    
}
