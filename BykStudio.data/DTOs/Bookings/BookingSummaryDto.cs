using System;
using System.Collections.Generic;
using System.Text;

namespace BykStudio.data.DTOs
{
    public class BookingSummaryDto
    {
        public string ResourceName { get; set; }   // e.g., room name or makeup table name
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
