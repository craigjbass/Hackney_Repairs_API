using System.Collections.Generic;
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
		Task<IEnumerable<UHWorkOrder>> GetWorkOrderByPropertyReference(string propertyReference);
        Task<IEnumerable<Note>> GetNoteFeed(int startId, string noteTarget, int size);
        Task<IEnumerable<string>> GetDistinctNoteKeyObjects();
    }
}
