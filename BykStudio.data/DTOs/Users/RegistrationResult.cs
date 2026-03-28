namespace BykStudio.data.DTOs
{
    public class RegistrationResult
    {
        public bool Success { get; set; }
        public RegisterResponse? Response { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
