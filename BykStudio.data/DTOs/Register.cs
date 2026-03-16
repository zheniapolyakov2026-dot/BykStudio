using System.ComponentModel.DataAnnotations;

namespace BykStudio.data.DTOs
{
    public class Register
    {
        [Required(ErrorMessage = "Пожалуйста, введите имя")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите email")]
        [EmailAddress(ErrorMessage = "Неверный формат email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите пароль")]
        [MinLength(4, ErrorMessage = "Пароль должен быть не менее 4 символов")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите номер телефона")]
        [Phone(ErrorMessage = "Неверный формат номера")]
        public string Phone { get; set; } = string.Empty;
    }
}
