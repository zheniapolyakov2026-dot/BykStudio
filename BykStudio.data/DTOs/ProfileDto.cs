using System;
using System.Collections.Generic;
using System.Text;

namespace BykStudio.data.DTOs
{
    public class ProfileDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public List<BookingSummaryDto> Bookings { get; set; } = new();
    }

    public class BookingSummaryDto
    {
        public string? RoomName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class TransactionSummaryDto
    {
        public DateTime CreatedAt { get; set; }
        public int Points { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
