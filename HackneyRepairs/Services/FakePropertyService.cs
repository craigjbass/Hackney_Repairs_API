using System;
using System.Threading.Tasks;
using HackneyRepairs.Interfaces;
using HackneyRepairs.PropertyService;

namespace HackneyRepairs.Services
{
    public class FakePropertyService : IHackneyPropertyService
    {
        public Task<PropertyInfoResponse> GetPropertyListByPostCodeAsync(ListByPostCodeRequest request)
        {
            var response = new PropertyInfoResponse()
            {
                PropertyList = new PropertySummary[2],
                Success = true
            };
            var property1 = new PropertySummary()
            {
                ShortAddress = "Back Office, Robert House, 6 - 15 Florfield Road",
                PostCodeValue = "E8 1DT",
                PropertyReference = "1/525252525"
            };
            var property2 = new PropertySummary()
            {
                ShortAddress = "Meeting room, Maurice Bishop House, 17 Reading Lane",
                PostCodeValue = "E8 1DT",
                PropertyReference = "6/32453245   "
            };
            response.PropertyList[0] = property1;
            response.PropertyList[1] = property2;
            switch (request.PostCode)
            {
                case "E8 1DT":
                    return Task.Run(() => response);
                case "E8 2LT":
                    return Task.Run(() => new PropertyInfoResponse
                                            {
                                                Success = false,
                                                ErrorCode = 9903,
                                                ErrorMessage = "Master Password is Invalid.",
                                                PropertyList = null
                                            });
                default:
                    return Task.Run(() => new PropertyInfoResponse
                                            {
                                                PropertyList = new PropertySummary[0],
                                                Success = true
                                            });
            }
        }

        public Task<PropertyGetResponse> GetPropertyByRefAsync(ByPropertyRefRequest request)
        {
            switch (request.PropertyReference)
            {
                case "525252525":
                    return Task.Run(() => new PropertyGetResponse()
                    {
                        Property = new PropertyDto
                        {
                            ShortAddress = "Back Office, Robert House, 6 - 15 Florfield Road    ",
                            PostCodeValue = "E8 1DT",
                            Reference = "525252525"
                        },
                        Success = true
                    });
                case "425252525":
                    return Task.Run(() => new PropertyGetResponse()
                    {
                        Property = null,
                        Success = false,
                    });
                case "42525252512":
                    return Task.Run(() => new PropertyGetResponse()
                    {
                        Property = null,
                        Success = true,
                    });
                default:
                    var response = new PropertyGetResponse()
                    {
                        Property = new PropertyDto(),
                        Success = true
                    };
                    return Task.Run(() => response);
            }
        }

        public Task<bool> GetMaintainable(string reference)
        {
            return Task.Run(() => reference == "525252525");
        }

        public Task<PropertySummary[]> GetPropertyListByPostCode(string post_code)
        {
            var  PropertyList= new PropertySummary[2];
            PropertySummary[] emptyPropertyList;
            var property1 = new PropertySummary()
            {
                ShortAddress = "Back Office, Robert House, 6 - 15 Florfield Road",
                PostCodeValue = "E8 1DT",
                PropertyReference = "1/525252525"
            };
            var property2 = new PropertySummary()
            {
                ShortAddress = "Meeting room, Maurice Bishop House, 17 Reading Lane",
                PostCodeValue = "E8 1DT",
                PropertyReference = "6/32453245   "
            };
            PropertyList[0] = property1;
            PropertyList[1] = property2;
            switch (post_code)
            {
                case "E8 1DT":
                    return Task.Run(() => PropertyList);
                case "E8 2LN":
                    emptyPropertyList = null;
                    return Task.Run(() => emptyPropertyList);
                 default:
                    emptyPropertyList = new PropertySummary[0];
                    return Task.Run(() => emptyPropertyList);
            }
        }
    }
}