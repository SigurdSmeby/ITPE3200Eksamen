namespace Sub_Application_1.DTOs
{
    public class RegisterDto
    {        
        public required string Username { get; set; } 
        
        public required string Email { get; set; }
        
        public required string Password { get; set; }
        public required string confirmPassword { get; set; }
    }
}
