namespace BykStudio.data.DTOs
{
    public class TinkoffPaymentResponse
    {
        public bool Success { get; set; }
        public string PaymentId { get; set; }
        public string PaymentURL { get; set; }
        public string ErrorCode { get; set; }
        public string Message { get; set; }
    }
}
