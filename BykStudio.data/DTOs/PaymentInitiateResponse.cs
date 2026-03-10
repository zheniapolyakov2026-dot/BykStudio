namespace BykStudio.data.DTOs
{
    public class PaymentInitiateResponse
    {
        public Guid PaymentId { get; set; }
        public string PaymentUrl { get; set; } = string.Empty;
    }
}
