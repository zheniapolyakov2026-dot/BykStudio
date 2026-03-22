namespace BykStudio.data.DTOs
{
    public class LoginResult
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
