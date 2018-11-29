using System;
using HackneyRepairs.Actions;
using HackneyRepairs.Interfaces;
using HackneyRepairs.Services;
using HackneyRepairs.Tests;

namespace HackneyRepairs.Factories
{
    public class HackneyNotesServiceFactory
    {
        public IHackneyNotesService build(IUhwRepository uhwRepository, ILoggerAdapter<NoteActions> logger)
        {
            if (TestStatus.IsRunningInTests == false)
            {
                return new HackneyNotesService(uhwRepository, logger);
            }
            else
            {
                return new FakeNotesService();
            }
        }
    }
}
