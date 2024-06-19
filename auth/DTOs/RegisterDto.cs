using System.ComponentModel.DataAnnotations;

namespace auth.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Имя пользователя уже существует")]
        public string Username { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        //[Phone]
        public string? PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
