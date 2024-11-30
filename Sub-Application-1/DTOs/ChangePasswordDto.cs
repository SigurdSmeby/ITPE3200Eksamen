namespace Sub_Application_1.DTOs
{
    public class ChangePasswordDto
    {
        // all fields are required, because the user needs all theese fields to register
        public required string CurrentPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}
