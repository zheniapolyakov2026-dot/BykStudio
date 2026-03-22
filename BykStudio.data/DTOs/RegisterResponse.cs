namespace BykStudio.data.DTOs
{
    public class RegisterResponse
    {
        public string Message { get; set; } = string.Empty;
        public string? ConfirmationUrl { get; set; }   // can keep for backward compatibility
        public string? UserId { get; set; }
        public string? Code { get; set; }              // the encoded confirmation code
    }
}
