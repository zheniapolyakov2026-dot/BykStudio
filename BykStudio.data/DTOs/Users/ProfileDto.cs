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
        public List<BookingSummaryDto> MakeupBookings { get; set; } = new();
    }
}
