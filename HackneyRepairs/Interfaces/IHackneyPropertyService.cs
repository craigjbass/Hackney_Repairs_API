using System.Threading.Tasks;
using HackneyRepairs.PropertyService;

namespace HackneyRepairs.Interfaces
{
    public interface IHackneyPropertyService
    {
        Task<PropertyInfoResponse> GetPropertyListByPostCodeAsync(ListByPostCodeRequest request);

        Task<PropertySummary[]> GetPropertyListByPostCode(string post_code);

        Task<PropertyGetResponse> GetPropertyByRefAsync(ByPropertyRefRequest request);

        Task<bool> GetMaintainable(string reference);
    }
}