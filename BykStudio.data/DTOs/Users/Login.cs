using System.ComponentModel.DataAnnotations;

namespace BykStudio.data.DTOs
{
    public class Login
    {
        [Required(ErrorMessage = "Введите email")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите пароль")]
        public string Password { get; set; } = string.Empty;
    }
}
