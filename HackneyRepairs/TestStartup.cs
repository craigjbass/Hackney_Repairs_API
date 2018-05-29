using HackneyRepairs.Tests;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HackneyRepairs
{
    public class TestStartup : Startup
    {
        public TestStartup(IHostingEnvironment env) : base(env)
        {
            TestStatus.IsRunningInTests = true;
        }
    }
}
