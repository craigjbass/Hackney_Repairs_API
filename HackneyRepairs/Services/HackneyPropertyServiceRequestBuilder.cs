using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using HackneyRepairs.Formatters;
using HackneyRepairs.Interfaces;
using HackneyRepairs.PropertyService;
using HackneyRepairs.Tests.Actions;

namespace HackneyRepairs.Services
{
    public class HackneyPropertyServiceRequestBuilder : IHackneyPropertyServiceRequestBuilder
    {
        private NameValueCollection _configuration;
        private PostcodeFormatter _postcodeFormatter;
        public HackneyPropertyServiceRequestBuilder(NameValueCollection configuration, PostcodeFormatter postcodeFormatter)
        {
            _configuration = configuration;
            _postcodeFormatter = postcodeFormatter;
        }
        public ListByPostCodeRequest BuildListByPostCodeRequest(string postcode)
        {
            var formattedPostcode = _postcodeFormatter.FormatPostcode(postcode);
            return new ListByPostCodeRequest
            {
                PostCode = formattedPostcode,
                DirectUser = GetUserCredentials(),
                SourceSystem = GetUhSourceSystem()
            };
        }

        public ByPropertyRefRequest BuildByPropertyRefRequest(string reference)
        {
            return new ByPropertyRefRequest
            {
                PropertyReference = reference,
                DirectUser = GetUserCredentials(),
                SourceSystem = GetUhSourceSystem()
            };
        }

        private UserCredential GetUserCredentials()
        {
            return new UserCredential
            {
                UserName = _configuration.Get("UHUsername"),
                UserPassword = _configuration.Get("UHPassword")
            };
        }

        private string GetUhSourceSystem()
        {
            return _configuration.Get("UHSourceSystem");
        }
    }
}
