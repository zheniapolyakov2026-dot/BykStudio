namespace BykStudio.data.DTOs
{
    public class PaymentInitiationResult
    {
        public Guid PaymentId { get; set; }
        public string PaymentUrl { get; set; } = string.Empty;
    }
}
