using System;
namespace HackneyRepairs.Models
{
    public class Note
    {
        public string WorkOrderReference { get; set; }
        public int NoteId { get; set; }
        public string Text { get; set; }
        public DateTime LoggedAt { get; set; }
        public string LoggedBy { get; set; }
    }
}
