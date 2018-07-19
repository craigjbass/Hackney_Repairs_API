using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HackneyRepairs.PropertyService;

namespace HackneyRepairs.Interfaces
{
    public interface IHackneyPropertyServiceRequestBuilder
    {
        string BuildListByPostCodeRequest(string postcode);

        ByPropertyRefRequest BuildByPropertyRefRequest(string reference);
    }
}
