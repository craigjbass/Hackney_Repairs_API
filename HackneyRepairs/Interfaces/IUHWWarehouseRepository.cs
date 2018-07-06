using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HackneyRepairs.Models;
using HackneyRepairs.PropertyService;

namespace HackneyRepairs.Interfaces
{
    public interface IUHWWarehouseRepository
    {
        Task<object> GetTagReferencenumber(string hackneyhomesId);
        Task<PropertySummary[]> GetPropertyListByPostCode(string post_code);
        Task<PropertyDetails> GetPropertyDetailsByReference(string reference);
    }
}
