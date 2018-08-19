using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HackneyRepairs.Models
{
    public class RepairRequest
    {
		public string repairRequestReference { get; set; }
        public string Priority { get; set; }
        public string PropertyReference { get; set; }
        public string ProblemDescription { get; set; }
        public RepairRequestContact Contact { get; set; }
        public List<WorkOrder> WorkOrders { get; set; }
    }

    public class RepairRequestContact
    {
        public string Name { get; set; }
        public string TelephoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string CallbackTime { get; set; }
    }
}
