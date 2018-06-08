using System;
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
            Environment.SetEnvironmentVariable("UhtDb", "connectionString=Test");
            Environment.SetEnvironmentVariable("UhwDb", "connectionString=Test");
            Environment.SetEnvironmentVariable("UhSorSupplierMapping", "08500820,H01|20040010,H01|20040020,H01|20040060,H01|20040310,H01|20060020,H01|20060030,H01|20110010,H01|48000000,H05|PRE00001,H02");
        }
    }
}
