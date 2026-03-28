namespace BykStudio.data.DTOs
{
    public abstract class BookingResultBase
    {
        public Guid BookingId { get; set; }
        public decimal TotalPrice { get; set; }
        public bool RequiresContactInfo { get; set; }
        public bool RequiresPayment { get; set; }
        public string? Message { get; set; }
    }

    public class CreateBookingResponse : BookingResultBase { }
    public class MakeupBookingResult : BookingResultBase { }
}
