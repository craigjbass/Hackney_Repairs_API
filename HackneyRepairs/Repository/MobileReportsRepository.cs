using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HackneyRepairs.Repository
{
    public static class MobileReportsRepository
    {
        private static string ServerName = Environment.GetEnvironmentVariable("MobileReportsServerName");
        private static string MountedPath = Environment.GetEnvironmentVariable("MobileReportsMountedPath");

        public static IEnumerable<string> GetReports(string servitorReference)
        {
            EnsureResourceAccess();

            var processedReports = Directory.GetFiles(MountedPath + "Processed/", $"Works Order_{servitorReference}*").ToList();
            if (processedReports.Any())
            {
                var unprocessedReports = Directory.GetFiles(MountedPath + "Unprocessed/", $"Works Order_{servitorReference}*").ToList();
                processedReports.InsertRange(0, unprocessedReports);
                processedReports = FormatReportStrings(processedReports).ToList();
            }
            return processedReports;
        }

        static IEnumerable<string> FormatReportStrings(IEnumerable<string> mobileReports)
        {
            var formattedMobileReportStrings = new List<string>();
            foreach (string report in mobileReports.ToList())
            {
                formattedMobileReportStrings.Add(report.Replace("/", @"\").Replace("volumes", "\\" + ServerName));
            }
            return formattedMobileReportStrings;
        }

        static void EnsureResourceAccess()
        {
            var directory = new DirectoryInfo(MountedPath);
            if (!directory.Exists)
            {
                throw new MobileReportsConnectionException();
            }
        }
    }

    public class MobileReportsConnectionException : Exception { }
}
