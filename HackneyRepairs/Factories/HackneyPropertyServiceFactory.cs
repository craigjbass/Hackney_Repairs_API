using HackneyRepairs.Actions;
using HackneyRepairs.Interfaces;
using HackneyRepairs.Tests;

namespace HackneyRepairs.Factories
{
    internal class HackneyPropertyServiceFactory
    {
        public IHackneyPropertyService build(IUhtRepository uhtRepository, ILoggerAdapter<PropertyActions> logger)
        {
           

            if (TestStatus.IsRunningInTests == false)
            {
                return new Services.HackneyPropertyService(uhtRepository, logger);
            }
            else
            {
                return new Services.FakePropertyService();
            }

        }
    }
}