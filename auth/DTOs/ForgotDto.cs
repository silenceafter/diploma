using System.ComponentModel.DataAnnotations;

namespace auth.DTOs
{
    public class ForgotDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
    }
}
