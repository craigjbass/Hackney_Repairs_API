using System;
namespace HackneyRepairs.Models
{
    public class DetailedNote : Note 
    {
        public string WorkOrderReference { get; set; }
        public int NoteId { get; set; }
    }
}
