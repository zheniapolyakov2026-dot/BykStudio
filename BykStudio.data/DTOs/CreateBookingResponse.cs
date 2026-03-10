namespace BykStudio.data.DTOs
{
    public class CreateBookingResponse
    {
        public Guid BookingId { get; set; }
        public decimal TotalPrice { get; set; }
        public int PointsRedeemed { get; set; }
        public bool RequiresContactInfo { get; set; }
        public bool RequiresPayment { get; set; }
        public string? Message { get; set; }
    }
}
