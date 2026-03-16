namespace BykStudio.data.DTOs
{
    public class RegisterResponse
    {
        public string Message { get; set; } = string.Empty;
        public string? ConfirmationUrl { get; set; }
    }
}
