using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HackneyRepairs.Models
{
    public class Slot
    {
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool BestSlot { get; set; }
        public bool Available { get; set; }
    }
}
