﻿using System;
namespace HackneyRepairs.Models
{
    public class NoteRequest
    {
        public string ObjectKey { get; set; }
        public string ObjectReference { get; set; }
        public string Text { get; set; }
    }
}
