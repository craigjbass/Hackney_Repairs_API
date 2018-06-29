using HackneyRepairs.Actions;
using HackneyRepairs.Interfaces;
using HackneyRepairs.Tests;

namespace HackneyRepairs.Factories
{
    internal class HackneyPropertyServiceFactory
    {
        public IHackneyPropertyService build(IUhtRepository uhtRepository, IUHWWarehouseRepository uHWWarehouseRepository, ILoggerAdapter<PropertyActions> logger)
        {
           

            if (TestStatus.IsRunningInTests == false)
            {
                return new Services.HackneyPropertyService(uhtRepository, uHWWarehouseRepository, logger);
            }
            else
            {
                return new Services.FakePropertyService();
            }

        }
    }
}