namespace BykStudio.data.DTOs
{
    public class MakeupBookingConfirmationDto
    {
        public Guid BookingId { get; set; }
        public string TableName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsGuestBooking { get; set; }
        public string? GuestName { get; set; }
        public string? GuestEmail { get; set; }
        public string? GuestPhone { get; set; }
    }
}
