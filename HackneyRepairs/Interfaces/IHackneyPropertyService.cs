using System.Threading.Tasks;
using HackneyRepairs.PropertyService;
using HackneyRepairs.Models;

namespace HackneyRepairs.Interfaces
{
    public interface IHackneyPropertyService
    {
        Task<PropertyInfoResponse> GetPropertyListByPostCodeAsync(ListByPostCodeRequest request);

        Task<PropertySummary[]> GetPropertyListByPostCode(string post_code);

        Task<PropertyGetResponse> GetPropertyByRefAsync(ByPropertyRefRequest request);

        Task<PropertyDetails> GetPropertyBlockByRef(string reference);

        Task<bool> GetMaintainable(string reference);
    }
}