﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RepairsService;

namespace HackneyRepairs.Models
{
    public class WorkOrder
    {
		public string workOrderReference { get; set; }
        public string SorCode { get; set; }
        public string SupplierRef { get; set; }
    }
}
