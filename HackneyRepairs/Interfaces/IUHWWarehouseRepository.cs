using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HackneyRepairs.PropertyService;

namespace HackneyRepairs.Interfaces
{
    public interface IUHWWarehouseRepository
    {
        Task<object> GetTagReferencenumber(string hackneyhomesId);
        Task<PropertySummary[]> GetPropertyListByPostCode(string post_code);
    }
}
