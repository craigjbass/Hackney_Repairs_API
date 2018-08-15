using System;
namespace HackneyRepairs.Entities
{
    public class UhtAppointmentEntity
    {
		public int visit_sid { get; set; }
        public int visiting_sid { get; set; }
        public int visitor_sid { get; set; }
        public int reference_sid { get; set; }
		public DateTime visit_appoint { get; set; }
        public bool visit_success { get; set; }
        public bool visit_carded { get; set; }
        public string visit_comment { get; set; }
		public int property_sid { get; set; }
        public DateTime visit_end { get; set; }
        public bool visit_slot { get; set; }
        public DateTime visit_prop_appointment { get; set; }
		public DateTime visit_prop_end { get; set; }
		public int visit_prop_duration { get; set; }
		public int visit_duration { get; set; }
		public int hadiary_sid { get; set; }
        public string visiting_table { get; set; }
		public string visitor_table { get; set; }
		public string reference_table { get; set; }
		public string visit_cat { get; set; }
		public string visit_outcome { get; set; }
		public string visit_outcometype { get; set; }
		public string visit_reason { get; set; }
        public DateTime visit_actual { get; set; }
		public string comp_avail { get; set; }
		public string comp_display { get; set; }
		public string contacttype_lrf { get; set; }
		public int visit_book_cat { get; set; }
		public string visit_reason_except { get; set; }
		public string visit_slot_type { get; set; }
        public int visit_processno { get; set; }
		public int visit_hhref { get; set; }
		public string visit_class { get; set; } 
		public string visit_type { get; set; }
    }
}
