﻿using System;
namespace HackneyRepairs.Models
{
    public class Note
    {
        public string Text { get; set; }
        public DateTime LoggedAt { get; set; }
        public string LoggedBy { get; set; }
    }
}
