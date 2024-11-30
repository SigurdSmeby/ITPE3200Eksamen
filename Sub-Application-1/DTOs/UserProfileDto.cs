namespace Sub_Application_1.DTOs
{
    public class UserProfileDto
    {
    public string? UserId { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? Bio { get; set; }
    public IFormFile? ProfilePicture { get; set; }

    public DateTime? DateJoined { get; set; }
    public int? TotalPosts { get; set; }
    public string? CurrentPassword { get; set; }
    public string? NewPassword { get; set; }
    public string? ConfirmPassword { get; set; }
    }
}
