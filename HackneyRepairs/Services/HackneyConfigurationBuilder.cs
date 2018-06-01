using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using HackneyRepairs.ExtensionMethods;

namespace HackneyRepairs.Services
{
    public class HackneyConfigurationBuilder
    {
        private NameValueCollection _configuration;
        public HackneyConfigurationBuilder(IDictionary<string, string> env_variables, NameValueCollection config_items)
        {
            if(env_variables!=null)
            {
                _configuration = env_variables.ToNameValueCollection();
            }
            else
            {
                _configuration = config_items;
            }
        }

        public NameValueCollection getConfiguration()
        {
            return _configuration;
        }


    }
}
