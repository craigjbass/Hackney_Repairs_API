using HackneyRepairs.Interfaces;
using HackneyRepairs.Tests;
using HackneyRepairs.Actions;

namespace HackneyRepairs.Factories
{
    public class HackneyRepairsServiceFactory
    {
        public IHackneyRepairsService build(IUhtRepository uhtRepository, IUhwRepository uhwRepository, ILoggerAdapter<RepairsActions> logger)
        {
            if (TestStatus.IsRunningInTests == false)
            {
                return new Services.HackneyRepairsService(uhtRepository, uhwRepository, logger);
            }
            else
            {
                return new Services.FakeRepairsService();
            }
        }

    }
}
