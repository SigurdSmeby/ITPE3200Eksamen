using System.ComponentModel.DataAnnotations;
namespace Sub_Application_1.DTOs
{
    public class RegisterDto
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string confirmPassword { get; set; }
    }
}
