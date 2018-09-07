using System.Threading.Tasks;
using HackneyRepairs.PropertyService;
using HackneyRepairs.Models;

namespace HackneyRepairs.Interfaces
{
    public interface IHackneyPropertyService
    {
        Task<PropertyLevelModel> GetPropertyLevelModel(string reference);
        Task<PropertyInfoResponse> GetPropertyListByPostCodeAsync(ListByPostCodeRequest request);
        Task<PropertySummary[]> GetPropertyListByPostCode(string post_code);
        Task<PropertyDetails> GetPropertyByRefAsync(string reference);
        Task<PropertyDetails> GetPropertyBlockByRef(string reference);
        Task<PropertyDetails> GetPropertyEstateByRef(string reference);
        Task<bool> GetMaintainable(string reference);
    }
}