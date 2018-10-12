using System;
using System.Collections.Generic;

namespace HackneyRepairs.Models
{
    public class UHWorkOrderWithMobileReports : UHWorkOrder
    {
        public IEnumerable<MobileReport> MobileReports { get; set; }
    }
}
