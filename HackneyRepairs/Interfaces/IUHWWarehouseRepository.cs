using System.Threading.Tasks;
using HackneyRepairs.Models;
using HackneyRepairs.PropertyService;

namespace HackneyRepairs.Interfaces
{
    public interface IUHWWarehouseRepository
    {
        Task<PropertyLevelModel> GetPropertyLevelInfo(string reference);
        Task<PropertySummary[]> GetPropertyListByPostCode(string post_code);
        Task<PropertyDetails> GetPropertyDetailsByReference(string reference);
        Task<PropertyDetails> GetPropertyBlockByReference(string reference);
        Task<PropertyDetails> GetPropertyEstateByReference(string reference);
    }
}
