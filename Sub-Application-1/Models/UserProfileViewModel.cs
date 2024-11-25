public class UserProfileViewModel
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? Bio { get; set; }
    public IFormFile? ProfilePicture { get; set; } // For file upload

    // For changing password
    public string? CurrentPassword { get; set; }
    public string? NewPassword { get; set; }
    public string? ConfirmPassword { get; set; }
}
