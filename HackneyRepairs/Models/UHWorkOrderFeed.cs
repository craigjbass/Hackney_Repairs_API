using System;
namespace HackneyRepairs.Models
{
    public class UHWorkOrderFeed
    {
        public string WorkOrderReference { get; set; }
        public string PropertyReference { get; set; }
        public string ProblemDescription { get; set; }
        public DateTime Created { get; set; }
    }
}
