using System.ComponentModel.DataAnnotations;
namespace Sub_Application_1.DTOs
{
    public class RegisterDto
    {
        
        public string Username { get; set; }
        
        public string Email { get; set; }
        
        public string Password { get; set; }
       
        public string confirmPassword { get; set; }
    }
}
