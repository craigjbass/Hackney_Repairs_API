using System;
namespace HackneyRepairs.DTOs
{
    public class RepairWithWorkOrderDto
    {
        public string rq_ref { get; set; }
        public string rq_problem { get; set; }
        public string rq_priority { get; set; }
        public string prop_ref { get; set; }
        public string rq_name { get; set; }
        public string rq_phone { get; set; }
        public string wo_ref { get; set; }
        public string sup_ref { get; set; }
        public string job_code { get; set; }
    }
}
