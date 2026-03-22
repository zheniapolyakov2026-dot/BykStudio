namespace BykStudio.data.DTOs
{
    public class MakeupBookingResult
    {
        public Guid BookingId { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Message { get; set; }
        public bool RequiresContactInfo { get; set; }
        public bool RequiresPayment { get; set; }
    }
}
